using SettingsService.Models;

namespace SettingsService.Services
{
    public interface ISettingsService
    {
        Task<ApiResponse<SettingsCompanyDetailsResponse>> GetCompanyDetailsAsync(int? userId);
        Task<ApiResponse<List<AdminUserResponse>>> GetAdminListAsync(int? userId, string? accountDNVId);
        Task<ApiResponse<List<MemberUserResponse>>> GetMemberListAsync(int? userId, string? accountDNVId);
        Task<ApiResponse<List<CountryResponse>>> GetCountriesAsync();
        Task<ApiResponse<PreferenceResponse>> GetPreferencesAsync(string objectType, string objectName, string pageName);
        Task<ApiResponse<CompanyDetailsUpdateResponse>> UpdateCompanyDetailsAsync(CompanyDetailsUpdateRequest input);
        Task<ApiResponse<UserPreferencesUpdateResponse>> UpdateUserPreferencesAsync(int userId, UserPreferencesUpdateRequest input);
        Task<ApiResponse<SystemPreferencesUpdateResponse>> UpdateSystemPreferencesAsync(SystemPreferencesUpdateRequest input);
        Task<ApiResponse<NotificationTemplateUpdateResponse>> UpdateNotificationTemplateAsync(NotificationTemplateUpdateRequest input);
    }
}
