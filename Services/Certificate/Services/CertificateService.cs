using CertificateService.Models;
using CertificateService.Repositories;

namespace CertificateService.Services
{
    public class CertificateService : ICertificateService
    {
        private readonly ICertificateRepository _repository;
        private readonly ILogger<CertificateService> _logger;

        public CertificateService(ICertificateRepository repository, ILogger<CertificateService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ApiResponse<List<CertificateListResponse>>> GetCertificateListAsync()
        {
            try
            {
                var data = (await _repository.GetCertificateListAsync()).ToList();
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load certificate list");
                return Failure<List<CertificateListResponse>>("Failed to load certificate list");
            }
        }

        public async Task<ApiResponse<CertificateDetailResponse>> GetCertificateDetailsAsync(int certificateId)
        {
            try
            {
                var data = await _repository.GetCertificateDetailsAsync(certificateId);
                if (data == null)
                {
                    return Failure<CertificateDetailResponse>("Certificate not found", "CERTIFICATE_NOT_FOUND");
                }

                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load certificate details");
                return Failure<CertificateDetailResponse>("Failed to load certificate details");
            }
        }

        public async Task<ApiResponse<List<CertificateSiteResponse>>> GetCertificateSitesAsync(int certificateId)
        {
            try
            {
                var data = (await _repository.GetCertificateSitesAsync(certificateId)).ToList();
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load certificate sites");
                return Failure<List<CertificateSiteResponse>>("Failed to load certificate sites");
            }
        }

        public async Task<ApiResponse<PreferenceResponse>> GetPreferencesAsync(string objectType, string objectName, string pageName)
        {
            try
            {
                var data = await _repository.GetPreferencesAsync(objectType, objectName, pageName);
                if (data == null)
                {
                    return Failure<PreferenceResponse>("Preferences not found", "PREFERENCES_NOT_FOUND");
                }

                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load preferences");
                return Failure<PreferenceResponse>("Failed to load preferences");
            }
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
                ErrorCode = errorCode ?? "ERR_CERTIFICATE"
            };
        }
    }
}
