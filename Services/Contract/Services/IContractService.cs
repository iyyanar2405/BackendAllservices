using ContractService.Models;

namespace ContractService.Services
{
    public interface IContractService
    {
        Task<ApiResponse<List<ContractListResponse>>> GetContractListAsync(int pageNumber, int pageSize, string? companyId, string? contractType);
        Task<ApiResponse<List<ServiceDetailsResponse>>> GetServiceListAsync();
        Task<ApiResponse<List<SiteDetailsResponse>>> GetMasterSiteListAsync();
        Task<ApiResponse<UserValidationResponse>> GetUserValidationAsync(string? userId, string? veracityId);
        Task<ApiResponse<UserProfileDetailsResponse>> GetUserProfileAsync(string? userId, string? veracityId);
        Task<ApiResponse<OverviewCardResponse>> GetOverviewCardDataAsync(OverviewFilter filter);
        Task<ApiResponse<List<OverviewCompanyServiceSiteFilterResult>>> GetOverviewCompanyServiceSiteFilterAsync();
        Task<ApiResponse<List<WidgetFinancialStatusResponse>>> GetOverviewFinancialStatusAsync();
        Task<ApiResponse<WidgetTrainingDataResponse>> GetWidgetForTrainingStatusAsync(string? userId);
        Task<ApiResponse<List<UpcomingAuditResponse>>> GetWidgetForUpcomingAuditAsync(DateTime? startDate, DateTime? endDate);
        Task<ApiResponse<PreferenceResponse>> GetPreferencesAsync(string objectType, string objectName, string pageName);
    }
}
