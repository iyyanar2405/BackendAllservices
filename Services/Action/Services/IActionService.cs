using ActionService.Models;

namespace ActionService.Services
{
    public interface IActionService
    {
        Task<ApiResponse<List<ActionFilterItem>>> GetActionCategoriesAsync(IEnumerable<int> companies, IEnumerable<int> services, IEnumerable<int> sites);
        Task<ApiResponse<List<ActionFilterItem>>> GetActionCompaniesAsync(IEnumerable<int> categories, IEnumerable<int> services, IEnumerable<int> sites);
        Task<ApiResponse<List<ActionFilterItem>>> GetActionServicesAsync(IEnumerable<int> companies, IEnumerable<int> categories, IEnumerable<int> sites);
        Task<ApiResponse<List<ActionSiteNode>>> GetActionSitesAsync(IEnumerable<int> companies, IEnumerable<int> categories, IEnumerable<int> services);
        Task<ApiResponse<ActionsPaginationResponse>> GetActionsAsync(IEnumerable<int> category, IEnumerable<int> company, IEnumerable<int> service, IEnumerable<int> site, bool isHighPriority, int pageNumber, int pageSize);
        Task<ApiResponse<ActionItem>> CreateActionAsync(CreateActionRequest request);
    }
}
