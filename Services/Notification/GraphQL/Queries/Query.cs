using NotificationService.Models;
using NotificationService.Services;

namespace NotificationService.GraphQL.Queries
{
    public class Query
    {
        private readonly INotificationService _service;

        public Query(INotificationService service)
        {
            _service = service;
        }

        [GraphQLName("notifications")]
        public Task<ApiResponse<NotificationPaginationResponse>> Notifications(
            List<int>? category,
            List<int>? company,
            List<int>? service,
            List<int>? site,
            int pageNumber,
            int pageSize)
        {
            return _service.GetNotificationsAsync(
                category ?? new List<int>(),
                company ?? new List<int>(),
                service ?? new List<int>(),
                site ?? new List<int>(),
                pageNumber,
                pageSize);
        }

        [GraphQLName("categoriesFilter")]
        public Task<ApiResponse<List<NotificationFilterItem>>> CategoriesFilter(
            List<int>? companies,
            List<int>? services,
            List<int>? sites)
        {
            return _service.GetCategoriesAsync(
                companies ?? new List<int>(),
                services ?? new List<int>(),
                sites ?? new List<int>());
        }

        [GraphQLName("servicesFilter")]
        public Task<ApiResponse<List<NotificationFilterItem>>> ServicesFilter(
            List<int>? companies,
            List<int>? categories,
            List<int>? sites)
        {
            return _service.GetServicesAsync(
                companies ?? new List<int>(),
                categories ?? new List<int>(),
                sites ?? new List<int>());
        }

        [GraphQLName("companiesFilter")]
        public Task<ApiResponse<List<NotificationFilterItem>>> CompaniesFilter(
            List<int>? categories,
            List<int>? services,
            List<int>? sites)
        {
            return _service.GetCompaniesAsync(
                categories ?? new List<int>(),
                services ?? new List<int>(),
                sites ?? new List<int>());
        }

        [GraphQLName("sitesFilter")]
        public Task<ApiResponse<List<NotificationSiteNode>>> SitesFilter(
            List<int>? companies,
            List<int>? categories,
            List<int>? services)
        {
            return _service.GetSitesAsync(
                companies ?? new List<int>(),
                categories ?? new List<int>(),
                services ?? new List<int>());
        }
    }
}
