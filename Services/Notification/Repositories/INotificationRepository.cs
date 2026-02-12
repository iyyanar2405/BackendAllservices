using NotificationService.Models;

namespace NotificationService.Repositories
{
    public interface INotificationRepository
    {
        Task<IReadOnlyList<NotificationFilterItem>> GetCategoriesAsync(IEnumerable<int> companies, IEnumerable<int> services, IEnumerable<int> sites);
        Task<IReadOnlyList<NotificationFilterItem>> GetServicesAsync(IEnumerable<int> companies, IEnumerable<int> categories, IEnumerable<int> sites);
        Task<IReadOnlyList<NotificationFilterItem>> GetCompaniesAsync(IEnumerable<int> categories, IEnumerable<int> services, IEnumerable<int> sites);
        Task<IReadOnlyList<NotificationSiteNode>> GetSitesAsync(IEnumerable<int> companies, IEnumerable<int> categories, IEnumerable<int> services);
        Task<NotificationPaginationResponse> GetNotificationsAsync(IEnumerable<int> category, IEnumerable<int> company, IEnumerable<int> service, IEnumerable<int> site, int pageNumber, int pageSize);
    }
}
