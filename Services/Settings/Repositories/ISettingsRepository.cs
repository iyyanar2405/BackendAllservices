using SettingsService.Models;

namespace SettingsService.Repositories
{
    public interface ISettingsRepository
    {
        Task<SettingsCompanyDetailsResponse?> GetCompanyDetailsAsync(int? userId);
        Task<IReadOnlyList<AdminUserResponse>> GetAdminListAsync(int? userId, string? accountDNVId);
        Task<IReadOnlyList<MemberUserResponse>> GetMemberListAsync(int? userId, string? accountDNVId);
        Task<IReadOnlyList<CountryResponse>> GetCountriesAsync();
        Task<PreferenceResponse?> GetPreferencesAsync(string objectType, string objectName, string pageName);
    }
}
