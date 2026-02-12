using NotificationService.Models;

namespace NotificationService.Services
{
    public interface INotificationService
    {
        Task<ApiResponse<List<NotificationFilterItem>>> GetCategoriesAsync(IEnumerable<int> companies, IEnumerable<int> services, IEnumerable<int> sites);
        Task<ApiResponse<List<NotificationFilterItem>>> GetServicesAsync(IEnumerable<int> companies, IEnumerable<int> categories, IEnumerable<int> sites);
        Task<ApiResponse<List<NotificationFilterItem>>> GetCompaniesAsync(IEnumerable<int> categories, IEnumerable<int> services, IEnumerable<int> sites);
        Task<ApiResponse<List<NotificationSiteNode>>> GetSitesAsync(IEnumerable<int> companies, IEnumerable<int> categories, IEnumerable<int> services);
        Task<ApiResponse<NotificationPaginationResponse>> GetNotificationsAsync(IEnumerable<int> category, IEnumerable<int> company, IEnumerable<int> service, IEnumerable<int> site, int pageNumber, int pageSize);
    }
}
