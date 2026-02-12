using SettingsService.Models;
using SettingsService.Repositories;

namespace SettingsService.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly ISettingsRepository _repository;
        private readonly ILogger<SettingsService> _logger;

        public SettingsService(ISettingsRepository repository, ILogger<SettingsService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ApiResponse<SettingsCompanyDetailsResponse>> GetCompanyDetailsAsync(int? userId)
        {
            try
            {
                var data = await _repository.GetCompanyDetailsAsync(userId);
                if (data == null)
                {
                    return Failure<SettingsCompanyDetailsResponse>("Company details not found", "COMPANY_NOT_FOUND");
                }

                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load company details");
                return Failure<SettingsCompanyDetailsResponse>("Failed to load company details");
            }
        }

        public async Task<ApiResponse<List<AdminUserResponse>>> GetAdminListAsync(int? userId, string? accountDNVId)
        {
            try
            {
                var data = (await _repository.GetAdminListAsync(userId, accountDNVId)).ToList();
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load admin list");
                return Failure<List<AdminUserResponse>>("Failed to load admin list");
            }
        }

        public async Task<ApiResponse<List<MemberUserResponse>>> GetMemberListAsync(int? userId, string? accountDNVId)
        {
            try
            {
                var data = (await _repository.GetMemberListAsync(userId, accountDNVId)).ToList();
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load member list");
                return Failure<List<MemberUserResponse>>("Failed to load member list");
            }
        }

        public async Task<ApiResponse<List<CountryResponse>>> GetCountriesAsync()
        {
            try
            {
                var data = (await _repository.GetCountriesAsync()).ToList();
                return Success(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load countries");
                return Failure<List<CountryResponse>>("Failed to load countries");
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

        public Task<ApiResponse<CompanyDetailsUpdateResponse>> UpdateCompanyDetailsAsync(CompanyDetailsUpdateRequest input)
        {
            _logger.LogWarning("UpdateCompanyDetailsAsync is not implemented");
            return Task.FromResult(Failure<CompanyDetailsUpdateResponse>("Update company details not implemented", "NOT_IMPLEMENTED"));
        }

        public Task<ApiResponse<UserPreferencesUpdateResponse>> UpdateUserPreferencesAsync(int userId, UserPreferencesUpdateRequest input)
        {
            _logger.LogWarning("UpdateUserPreferencesAsync is not implemented");
            return Task.FromResult(Failure<UserPreferencesUpdateResponse>("Update user preferences not implemented", "NOT_IMPLEMENTED"));
        }

        public Task<ApiResponse<SystemPreferencesUpdateResponse>> UpdateSystemPreferencesAsync(SystemPreferencesUpdateRequest input)
        {
            _logger.LogWarning("UpdateSystemPreferencesAsync is not implemented");
            return Task.FromResult(Failure<SystemPreferencesUpdateResponse>("Update system preferences not implemented", "NOT_IMPLEMENTED"));
        }

        public Task<ApiResponse<NotificationTemplateUpdateResponse>> UpdateNotificationTemplateAsync(NotificationTemplateUpdateRequest input)
        {
            _logger.LogWarning("UpdateNotificationTemplateAsync is not implemented");
            return Task.FromResult(Failure<NotificationTemplateUpdateResponse>("Update notification template not implemented", "NOT_IMPLEMENTED"));
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
                ErrorCode = errorCode ?? "ERR_SETTINGS"
            };
        }
    }
}
