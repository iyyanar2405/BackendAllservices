using ContractService.Models;
using ContractService.Services;

namespace ContractService.GraphQL.Queries
{
    public class Query
    {
        private readonly IContractService _service;

        public Query(IContractService service)
        {
            _service = service;
        }

        [GraphQLName("validateUser")]
        public Task<ApiResponse<UserValidationResponse>> ValidateUser(string? userId, string? veracityId)
        {
            return _service.GetUserValidationAsync(userId, veracityId);
        }

        [GraphQLName("userProfile")]
        public Task<ApiResponse<UserProfileDetailsResponse>> UserProfile(string? userId, string? veracityId)
        {
            return _service.GetUserProfileAsync(userId, veracityId);
        }

        [GraphQLName("masterSiteList")]
        public Task<ApiResponse<List<SiteDetailsResponse>>> MasterSiteList()
        {
            return _service.GetMasterSiteListAsync();
        }

        [GraphQLName("masterServiceList")]
        public Task<ApiResponse<List<ServiceDetailsResponse>>> MasterServiceList()
        {
            return _service.GetServiceListAsync();
        }

        [GraphQLName("viewCertificationQuicklinkCard")]
        public Task<ApiResponse<OverviewCardResponse>> ViewCertificationQuicklinkCard(OverviewFilter filter)
        {
            return _service.GetOverviewCardDataAsync(filter);
        }

        [GraphQLName("overviewCompanyServiceSiteFilter")]
        public Task<ApiResponse<List<OverviewCompanyServiceSiteFilterResult>>> OverviewCompanyServiceSiteFilter()
        {
            return _service.GetOverviewCompanyServiceSiteFilterAsync();
        }

        [GraphQLName("getWidgetforFinancials")]
        public Task<ApiResponse<List<WidgetFinancialStatusResponse>>> GetWidgetforFinancials()
        {
            return _service.GetOverviewFinancialStatusAsync();
        }

        [GraphQLName("widgetforTrainingStatus")]
        public Task<ApiResponse<WidgetTrainingDataResponse>> WidgetforTrainingStatus(string? userId)
        {
            return _service.GetWidgetForTrainingStatusAsync(userId);
        }

        [GraphQLName("getWidgetForUpcomingAudit")]
        public Task<ApiResponse<List<UpcomingAuditResponse>>> GetWidgetForUpcomingAudit(DateTime? startDate, DateTime? endDate)
        {
            return _service.GetWidgetForUpcomingAuditAsync(startDate, endDate);
        }

        [GraphQLName("preferences")]
        public Task<ApiResponse<PreferenceResponse>> Preferences(string objectType, string objectName, string pageName)
        {
            return _service.GetPreferencesAsync(objectType, objectName, pageName);
        }
    }
}
