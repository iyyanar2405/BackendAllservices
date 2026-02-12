using ActionService.Models;

namespace ActionService.Repositories
{
    public interface IActionRepository
    {
        Task<IReadOnlyList<ActionFilterItem>> GetActionCategoriesAsync(IEnumerable<int> companies, IEnumerable<int> services, IEnumerable<int> sites);
        Task<IReadOnlyList<ActionFilterItem>> GetActionCompaniesAsync(IEnumerable<int> categories, IEnumerable<int> services, IEnumerable<int> sites);
        Task<IReadOnlyList<ActionFilterItem>> GetActionServicesAsync(IEnumerable<int> companies, IEnumerable<int> categories, IEnumerable<int> sites);
        Task<IReadOnlyList<ActionSiteNode>> GetActionSitesAsync(IEnumerable<int> companies, IEnumerable<int> categories, IEnumerable<int> services);
        Task<ActionsPaginationResponse> GetActionsAsync(IEnumerable<int> category, IEnumerable<int> company, IEnumerable<int> service, IEnumerable<int> site, bool isHighPriority, int pageNumber, int pageSize);
        Task<ActionItem> CreateActionAsync(CreateActionRequest request);
    }
}
