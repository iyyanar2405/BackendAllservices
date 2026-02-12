using ContractService.Models;
using ContractService.Repositories;

namespace ContractService.Services
{
    public class ContractService : IContractService
    {
        private readonly IContractRepository _repository;
        private readonly ILogger<ContractService> _logger;

        public ContractService(IContractRepository repository, ILogger<ContractService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ApiResponse<List<ContractListResponse>>> GetContractListAsync(int pageNumber, int pageSize, string? companyId, string? contractType)
        {
            try
            {
                var data = (await _repository.GetContractListAsync(pageNumber, pageSize, companyId, contractType)).ToList();
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load contract list");
                return Failure<List<ContractListResponse>>("Failed to load contract list");
            }
        }

        public async Task<ApiResponse<List<ServiceDetailsResponse>>> GetServiceListAsync()
        {
            try
            {
                var data = (await _repository.GetServiceListAsync()).ToList();
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load service list");
                return Failure<List<ServiceDetailsResponse>>("Failed to load service list");
            }
        }

        public async Task<ApiResponse<List<SiteDetailsResponse>>> GetMasterSiteListAsync()
        {
            try
            {
                var data = (await _repository.GetMasterSiteListAsync()).ToList();
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load site list");
                return Failure<List<SiteDetailsResponse>>("Failed to load site list");
            }
        }

        public async Task<ApiResponse<UserValidationResponse>> GetUserValidationAsync(string? userId, string? veracityId)
        {
            try
            {
                var data = await _repository.GetUserValidationAsync(userId, veracityId);
                if (data == null)
                {
                    return Failure<UserValidationResponse>("User not found", "USER_NOT_FOUND");
                }

                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to validate user");
                return Failure<UserValidationResponse>("Failed to validate user");
            }
        }

        public async Task<ApiResponse<UserProfileDetailsResponse>> GetUserProfileAsync(string? userId, string? veracityId)
        {
            try
            {
                var data = await _repository.GetUserProfileAsync(userId, veracityId);
                if (data == null)
                {
                    return Failure<UserProfileDetailsResponse>("User profile not found", "USER_NOT_FOUND");
                }

                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load user profile");
                return Failure<UserProfileDetailsResponse>("Failed to load user profile");
            }
        }

        public async Task<ApiResponse<OverviewCardResponse>> GetOverviewCardDataAsync(OverviewFilter filter)
        {
            try
            {
                var data = await _repository.GetOverviewCardDataAsync(filter) ?? new OverviewCardResponse();
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load overview card data");
                return Failure<OverviewCardResponse>("Failed to load overview card data");
            }
        }

        public async Task<ApiResponse<List<OverviewCompanyServiceSiteFilterResult>>> GetOverviewCompanyServiceSiteFilterAsync()
        {
            try
            {
                var data = (await _repository.GetOverviewCompanyServiceSiteFilterAsync()).ToList();
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load overview filter data");
                return Failure<List<OverviewCompanyServiceSiteFilterResult>>("Failed to load overview filter data");
            }
        }

        public async Task<ApiResponse<List<WidgetFinancialStatusResponse>>> GetOverviewFinancialStatusAsync()
        {
            try
            {
                var data = (await _repository.GetOverviewFinancialStatusAsync()).ToList();
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load financial status data");
                return Failure<List<WidgetFinancialStatusResponse>>("Failed to load financial status data");
            }
        }

        public async Task<ApiResponse<WidgetTrainingDataResponse>> GetWidgetForTrainingStatusAsync(string? userId)
        {
            try
            {
                var data = await _repository.GetWidgetForTrainingStatusAsync(userId) ?? new WidgetTrainingDataResponse();
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load training status data");
                return Failure<WidgetTrainingDataResponse>("Failed to load training status data");
            }
        }

        public async Task<ApiResponse<List<UpcomingAuditResponse>>> GetWidgetForUpcomingAuditAsync(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var data = (await _repository.GetWidgetForUpcomingAuditAsync(startDate, endDate)).ToList();
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load upcoming audit data");
                return Failure<List<UpcomingAuditResponse>>("Failed to load upcoming audit data");
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
                ErrorCode = errorCode ?? "ERR_CONTRACT"
            };
        }
    }
}
