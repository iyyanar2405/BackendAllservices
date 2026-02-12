using ActionService.Models;
using ActionService.Services;

namespace ActionService.GraphQL.Queries
{
    public class Query
    {
        private readonly IActionService _service;

        public Query(IActionService service)
        {
            _service = service;
        }

        [GraphQLName("actions")]
        public Task<ApiResponse<ActionsPaginationResponse>> Actions(
            List<int>? category,
            List<int>? company,
            List<int>? service,
            List<int>? site,
            bool isHighPriority,
            int pageNumber,
            int pageSize)
        {
            return _service.GetActionsAsync(
                category ?? new List<int>(),
                company ?? new List<int>(),
                service ?? new List<int>(),
                site ?? new List<int>(),
                isHighPriority,
                pageNumber,
                pageSize);
        }

        [GraphQLName("actionCategoriesFilter")]
        public Task<ApiResponse<List<ActionFilterItem>>> ActionCategoriesFilter(
            List<int>? companies,
            List<int>? services,
            List<int>? sites)
        {
            return _service.GetActionCategoriesAsync(
                companies ?? new List<int>(),
                services ?? new List<int>(),
                sites ?? new List<int>());
        }

        [GraphQLName("actionCompaniesFilter")]
        public Task<ApiResponse<List<ActionFilterItem>>> ActionCompaniesFilter(
            List<int>? categories,
            List<int>? services,
            List<int>? sites)
        {
            return _service.GetActionCompaniesAsync(
                categories ?? new List<int>(),
                services ?? new List<int>(),
                sites ?? new List<int>());
        }

        [GraphQLName("actionServicesFilter")]
        public Task<ApiResponse<List<ActionFilterItem>>> ActionServicesFilter(
            List<int>? companies,
            List<int>? categories,
            List<int>? sites)
        {
            return _service.GetActionServicesAsync(
                companies ?? new List<int>(),
                categories ?? new List<int>(),
                sites ?? new List<int>());
        }

        [GraphQLName("actionSitesFilter")]
        public Task<ApiResponse<List<ActionSiteNode>>> ActionSitesFilter(
            List<int>? companies,
            List<int>? categories,
            List<int>? services)
        {
            return _service.GetActionSitesAsync(
                companies ?? new List<int>(),
                categories ?? new List<int>(),
                services ?? new List<int>());
        }
    }
}
