using CertificateService.Models;
using CertificateService.Models.Rest;
using RestCertificateListResponse = CertificateService.Models.Rest.CertificateListResponse;
using CertificateService.Repositories;

namespace CertificateService.Services
{
    public class CertificateRestService : ICertificateRestService
    {
        private readonly ICertificateRepository _repository;
        private readonly ILogger<CertificateRestService> _logger;

        public CertificateRestService(ICertificateRepository repository, ILogger<CertificateRestService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ApiResponse<RestCertificateListResponse>> GetCertificateListAsync(CertificateListRequest request)
        {
            try
            {
                var data = await _repository.GetCertificateListPageAsync(request);
                var response = BuildListResponse(data, request.PageNumber, request.PageSize);
                return Success(response, "Certificate list retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load certificate list");
                return Failure<RestCertificateListResponse>("Failed to load certificate list");
            }
        }

        public async Task<ApiResponse<RestCertificateListResponse>> SearchCertificatesAsync(CertificateSearchRequest request)
        {
            try
            {
                var data = await _repository.SearchCertificatesAsync(request);
                var response = BuildListResponse(data, request.PageNumber, request.PageSize);
                return Success(response, "Certificate list retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to search certificates");
                return Failure<RestCertificateListResponse>("Failed to search certificates");
            }
        }

        public async Task<ApiResponse<CertificateDetailFullResponse>> GetCertificateDetailsAsync(int certificateId)
        {
            try
            {
                var detail = await _repository.GetCertificateDetailsFullAsync(certificateId);
                if (detail == null)
                {
                    return Failure<CertificateDetailFullResponse>("Certificate not found", "CERTIFICATE_NOT_FOUND");
                }

                return Success(detail, "Certificate details retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load certificate details");
                return Failure<CertificateDetailFullResponse>("Failed to load certificate details");
            }
        }

        public async Task<ApiResponse<CertificateStatusUpdateResponse>> UpdateCertificateStatusAsync(int certificateId, UpdateCertificateStatusRequest request)
        {
            try
            {
                if (request.CertificateId != 0 && request.CertificateId != certificateId)
                {
                    return Failure<CertificateStatusUpdateResponse>("Certificate ID mismatch", "INVALID_CERTIFICATE_ID");
                }

                if (string.IsNullOrWhiteSpace(request.NewStatus))
                {
                    return Failure<CertificateStatusUpdateResponse>("Status is required", "INVALID_STATUS");
                }

                var result = await _repository.UpdateCertificateStatusAsync(certificateId, request);
                if (result == null)
                {
                    return Failure<CertificateStatusUpdateResponse>("Certificate not found", "CERTIFICATE_NOT_FOUND");
                }

                return Success(result, "Certificate status updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update certificate status");
                return Failure<CertificateStatusUpdateResponse>("Failed to update certificate status");
            }
        }

        private static RestCertificateListResponse BuildListResponse(CertificateListPageData data, int pageNumber, int pageSize)
        {
            var items = data.Items;
            ApplyComputedFields(items);

            var normalizedPageSize = pageSize <= 0 ? 10 : pageSize;
            var normalizedPageNumber = pageNumber <= 0 ? 1 : pageNumber;
            var hasNext = (normalizedPageNumber * normalizedPageSize) < data.TotalCount;

            return new RestCertificateListResponse
            {
                Certificates = items,
                Summary = new CertificateListSummary
                {
                    TotalCertificates = data.TotalCount,
                    ByStatus = NormalizeStatusCounts(data.StatusCounts),
                    ExpiringWithin30Days = data.ExpiringWithin30Days,
                    ExpiringWithin90Days = data.ExpiringWithin90Days
                },
                TotalCount = data.TotalCount,
                PageNumber = normalizedPageNumber,
                PageSize = normalizedPageSize,
                HasNextPage = hasNext,
                HasPreviousPage = normalizedPageNumber > 1
            };
        }

        private static void ApplyComputedFields(IEnumerable<CertificateListItemResponse> items)
        {
            var today = DateTime.UtcNow.Date;
            foreach (var item in items)
            {
                if (!item.ValidUntil.HasValue)
                {
                    continue;
                }

                var daysUntil = (int)Math.Floor((item.ValidUntil.Value.Date - today).TotalDays);
                item.DaysUntilExpiry = daysUntil;

                if (daysUntil < 0)
                {
                    item.RenewalStatus = "Expired";
                }
                else if (daysUntil <= 90)
                {
                    item.RenewalStatus = "Pending";
                }
                else
                {
                    item.RenewalStatus = "Not Required";
                }
            }
        }

        private static Dictionary<string, int> NormalizeStatusCounts(Dictionary<string, int> statusCounts)
        {
            var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var pair in statusCounts)
            {
                var key = pair.Key?.Trim() ?? "Unknown";
                var normalized = key switch
                {
                    "Active" => "valid",
                    "In Progress" => "inProgress",
                    "Suspended" => "suspended",
                    "Expired" => "expired",
                    "Withdrawn" => "withdrawn",
                    "Cancelled" => "cancelled",
                    _ => NormalizeKey(key)
                };

                result[normalized] = pair.Value;
            }

            return result;
        }

        private static string NormalizeKey(string key)
        {
            var trimmed = key.Replace(" ", string.Empty);
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                return "unknown";
            }

            return char.ToLowerInvariant(trimmed[0]) + trimmed[1..];
        }

        private static ApiResponse<T> Success<T>(T data, string message)
        {
            return new ApiResponse<T>
            {
                Data = data,
                IsSuccess = true,
                Message = message,
                ErrorCode = string.Empty
            };
        }

        private static ApiResponse<T> Failure<T>(string message, string? errorCode = null)
        {
            return new ApiResponse<T>
            {
                Data = default,
                IsSuccess = false,
                Message = message,
                ErrorCode = errorCode ?? "ERR_CERTIFICATE"
            };
        }
    }
}
