using ContractService.Models;

namespace ContractService.Repositories
{
    public interface IContractRepository
    {
        Task<IReadOnlyList<ContractListResponse>> GetContractListAsync(int pageNumber, int pageSize, string? companyId, string? contractType);
        Task<IReadOnlyList<ServiceDetailsResponse>> GetServiceListAsync();
        Task<IReadOnlyList<SiteDetailsResponse>> GetMasterSiteListAsync();
        Task<UserValidationResponse?> GetUserValidationAsync(string? userId, string? veracityId);
        Task<UserProfileDetailsResponse?> GetUserProfileAsync(string? userId, string? veracityId);
        Task<OverviewCardResponse?> GetOverviewCardDataAsync(OverviewFilter filter);
        Task<IReadOnlyList<OverviewCompanyServiceSiteFilterResult>> GetOverviewCompanyServiceSiteFilterAsync();
        Task<IReadOnlyList<WidgetFinancialStatusResponse>> GetOverviewFinancialStatusAsync();
        Task<WidgetTrainingDataResponse?> GetWidgetForTrainingStatusAsync(string? userId);
        Task<IReadOnlyList<UpcomingAuditResponse>> GetWidgetForUpcomingAuditAsync(DateTime? startDate, DateTime? endDate);
        Task<PreferenceResponse?> GetPreferencesAsync(string objectType, string objectName, string pageName);
    }
}
