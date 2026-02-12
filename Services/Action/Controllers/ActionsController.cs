using ActionService.Models;
using ActionService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ActionService.Controllers
{
    [ApiController]
    [Route("api/actions")]
    public class ActionsController : ControllerBase
    {
        private readonly IActionService _service;

        public ActionsController(IActionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<ActionsPaginationResponse>>> GetActions(
            [FromQuery] List<int>? category,
            [FromQuery] List<int>? company,
            [FromQuery] List<int>? service,
            [FromQuery] List<int>? site,
            [FromQuery] bool isHighPriority = false,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var response = await _service.GetActionsAsync(
                category ?? new List<int>(),
                company ?? new List<int>(),
                service ?? new List<int>(),
                site ?? new List<int>(),
                isHighPriority,
                pageNumber,
                pageSize);

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ActionItem>>> CreateAction([FromBody] CreateActionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Action))
            {
                return BadRequest(new ApiResponse<ActionItem>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Action is required",
                    ErrorCode = "ERR_ACTIONS_VALIDATION"
                });
            }

            var response = await _service.CreateActionAsync(request);
            return Ok(response);
        }

        [HttpGet("filters/categories")]
        public async Task<ActionResult<ApiResponse<List<ActionFilterItem>>>> GetCategories(
            [FromQuery] List<int>? companies,
            [FromQuery] List<int>? services,
            [FromQuery] List<int>? sites)
        {
            var response = await _service.GetActionCategoriesAsync(
                companies ?? new List<int>(),
                services ?? new List<int>(),
                sites ?? new List<int>());

            return Ok(response);
        }

        [HttpGet("filters/companies")]
        public async Task<ActionResult<ApiResponse<List<ActionFilterItem>>>> GetCompanies(
            [FromQuery] List<int>? categories,
            [FromQuery] List<int>? services,
            [FromQuery] List<int>? sites)
        {
            var response = await _service.GetActionCompaniesAsync(
                categories ?? new List<int>(),
                services ?? new List<int>(),
                sites ?? new List<int>());

            return Ok(response);
        }

        [HttpGet("filters/services")]
        public async Task<ActionResult<ApiResponse<List<ActionFilterItem>>>> GetServices(
            [FromQuery] List<int>? companies,
            [FromQuery] List<int>? categories,
            [FromQuery] List<int>? sites)
        {
            var response = await _service.GetActionServicesAsync(
                companies ?? new List<int>(),
                categories ?? new List<int>(),
                sites ?? new List<int>());

            return Ok(response);
        }

        [HttpGet("filters/sites")]
        public async Task<ActionResult<ApiResponse<List<ActionSiteNode>>>> GetSites(
            [FromQuery] List<int>? companies,
            [FromQuery] List<int>? categories,
            [FromQuery] List<int>? services)
        {
            var response = await _service.GetActionSitesAsync(
                companies ?? new List<int>(),
                categories ?? new List<int>(),
                services ?? new List<int>());

            return Ok(response);
        }
    }
}
