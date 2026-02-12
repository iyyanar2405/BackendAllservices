using SettingsService.Models;
using SettingsService.Services;

namespace SettingsService.GraphQL.Queries
{
    public class Query
    {
        private readonly ISettingsService _service;

        public Query(ISettingsService service)
        {
            _service = service;
        }

        [GraphQLName("userCompanyDetails")]
        public Task<ApiResponse<SettingsCompanyDetailsResponse>> UserCompanyDetails(int? userId)
        {
            return _service.GetCompanyDetailsAsync(userId);
        }

        [GraphQLName("adminList")]
        public Task<ApiResponse<List<AdminUserResponse>>> AdminList(string? accountDNVId, int? userId)
        {
            return _service.GetAdminListAsync(userId, accountDNVId);
        }

        [GraphQLName("memberList")]
        public Task<ApiResponse<List<MemberUserResponse>>> MemberList(string? accountDNVId, int? userId)
        {
            return _service.GetMemberListAsync(userId, accountDNVId);
        }

        [GraphQLName("getCountries")]
        public Task<ApiResponse<List<CountryResponse>>> GetCountries()
        {
            return _service.GetCountriesAsync();
        }

        [GraphQLName("preferences")]
        public Task<ApiResponse<PreferenceResponse>> Preferences(string objectType, string objectName, string pageName)
        {
            return _service.GetPreferencesAsync(objectType, objectName, pageName);
        }
    }
}
