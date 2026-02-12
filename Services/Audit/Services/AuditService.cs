using AuditService.Models;
using AuditService.Repositories;

namespace AuditService.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditRepository _repository;
        private readonly ILogger<AuditService> _logger;

        public AuditService(IAuditRepository repository, ILogger<AuditService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ApiResponse<List<AuditListResponse>>> GetAuditListAsync()
        {
            try
            {
                var data = (await _repository.GetAuditListAsync()).ToList();
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load audit list");
                return Failure<List<AuditListResponse>>("Failed to load audit list");
            }
        }

        public async Task<ApiResponse<AuditDetailResponse>> GetAuditDetailsAsync(int auditId)
        {
            try
            {
                var data = await _repository.GetAuditDetailsAsync(auditId);
                if (data == null)
                {
                    return Failure<AuditDetailResponse>("Audit not found", "AUDIT_NOT_FOUND");
                }

                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load audit details");
                return Failure<AuditDetailResponse>("Failed to load audit details");
            }
        }

        public async Task<ApiResponse<List<AuditFindingListResponse>>> GetAuditFindingsAsync(int auditId)
        {
            try
            {
                var data = (await _repository.GetAuditFindingsAsync(auditId)).ToList();
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load audit findings");
                return Failure<List<AuditFindingListResponse>>("Failed to load audit findings");
            }
        }

        public async Task<ApiResponse<List<AuditSiteResponse>>> GetAuditSitesAsync(int auditId)
        {
            try
            {
                var data = (await _repository.GetAuditSitesAsync(auditId)).ToList();
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load audit sites");
                return Failure<List<AuditSiteResponse>>("Failed to load audit sites");
            }
        }

        public async Task<ApiResponse<List<SubAuditResponse>>> GetSubAuditsAsync(int auditId)
        {
            try
            {
                var data = (await _repository.GetSubAuditsAsync(auditId)).ToList();
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load sub audits");
                return Failure<List<SubAuditResponse>>("Failed to load sub audits");
            }
        }

        public Task<ApiResponse<AuditDaysGridResponse>> GetAuditDaysGridAsync(string startDate, string endDate, List<int> companies, List<string> services, List<int> sites)
        {
            return _repository.GetAuditDaysGridAsync(startDate, endDate, companies, services, sites);
        }

        public Task<ApiResponse<AuditDaysByServiceResponse>> GetAuditDaysByServiceAsync(AuditDaysFilter filters)
        {
            return _repository.GetAuditDaysByServiceAsync(filters);
        }

        public Task<ApiResponse<AuditDaysByMonthAndServiceResponse>> GetAuditDaysByMonthAndServiceAsync(AuditDaysByMonthFilter filters)
        {
            return _repository.GetAuditDaysByMonthAndServiceAsync(filters);
        }

        private static ApiResponse<T> Success<T>(T data)
        {
            return new ApiResponse<T>
            {
                Data = data,
                IsSuccess = true,
                Message = "Success",
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
                ErrorCode = errorCode ?? "ERR_AUDIT"
            };
        }
    }
}
