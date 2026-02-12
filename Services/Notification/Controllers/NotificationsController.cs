using Microsoft.AspNetCore.Mvc;
using NotificationService.Models;
using NotificationService.Services;

namespace NotificationService.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationsController(INotificationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<NotificationPaginationResponse>>> GetNotifications(
            [FromQuery] List<int>? category,
            [FromQuery] List<int>? company,
            [FromQuery] List<int>? service,
            [FromQuery] List<int>? site,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var response = await _service.GetNotificationsAsync(
                category ?? new List<int>(),
                company ?? new List<int>(),
                service ?? new List<int>(),
                site ?? new List<int>(),
                pageNumber,
                pageSize);

            return Ok(response);
        }

        [HttpGet("filters/categories")]
        public async Task<ActionResult<ApiResponse<List<NotificationFilterItem>>>> GetCategories(
            [FromQuery] List<int>? companies,
            [FromQuery] List<int>? services,
            [FromQuery] List<int>? sites)
        {
            var response = await _service.GetCategoriesAsync(
                companies ?? new List<int>(),
                services ?? new List<int>(),
                sites ?? new List<int>());

            return Ok(response);
        }

        [HttpGet("filters/services")]
        public async Task<ActionResult<ApiResponse<List<NotificationFilterItem>>>> GetServices(
            [FromQuery] List<int>? companies,
            [FromQuery] List<int>? categories,
            [FromQuery] List<int>? sites)
        {
            var response = await _service.GetServicesAsync(
                companies ?? new List<int>(),
                categories ?? new List<int>(),
                sites ?? new List<int>());

            return Ok(response);
        }

        [HttpGet("filters/companies")]
        public async Task<ActionResult<ApiResponse<List<NotificationFilterItem>>>> GetCompanies(
            [FromQuery] List<int>? categories,
            [FromQuery] List<int>? services,
            [FromQuery] List<int>? sites)
        {
            var response = await _service.GetCompaniesAsync(
                categories ?? new List<int>(),
                services ?? new List<int>(),
                sites ?? new List<int>());

            return Ok(response);
        }

        [HttpGet("filters/sites")]
        public async Task<ActionResult<ApiResponse<List<NotificationSiteNode>>>> GetSites(
            [FromQuery] List<int>? companies,
            [FromQuery] List<int>? categories,
            [FromQuery] List<int>? services)
        {
            var response = await _service.GetSitesAsync(
                companies ?? new List<int>(),
                categories ?? new List<int>(),
                services ?? new List<int>());

            return Ok(response);
        }
    }
}
