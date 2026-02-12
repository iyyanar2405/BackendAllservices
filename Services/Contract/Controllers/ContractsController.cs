using ContractService.Models;
using ContractService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContractService.Controllers
{
    [ApiController]
    [Route("api/contracts")]
    public class ContractsController : ControllerBase
    {
        private readonly IContractService _service;

        public ContractsController(IContractService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ContractListResponse>>>> GetContracts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? companyId = null,
            [FromQuery] string? contractType = null)
        {
            var response = await _service.GetContractListAsync(pageNumber, pageSize, companyId, contractType);
            return Ok(response);
        }

        [HttpGet("services")]
        public async Task<ActionResult<ApiResponse<List<ServiceDetailsResponse>>>> GetServiceList()
        {
            var response = await _service.GetServiceListAsync();
            return Ok(response);
        }

        [HttpGet("sites")]
        public async Task<ActionResult<ApiResponse<List<SiteDetailsResponse>>>> GetSiteList()
        {
            var response = await _service.GetMasterSiteListAsync();
            return Ok(response);
        }

        [HttpGet("users/validate")]
        public async Task<ActionResult<ApiResponse<UserValidationResponse>>> GetUserValidation(
            [FromQuery] string? userId,
            [FromQuery] string? veracityId)
        {
            var response = await _service.GetUserValidationAsync(userId, veracityId);
            return Ok(response);
        }

        [HttpGet("users/profile")]
        public async Task<ActionResult<ApiResponse<UserProfileDetailsResponse>>> GetUserProfile(
            [FromQuery] string? userId,
            [FromQuery] string? veracityId)
        {
            var response = await _service.GetUserProfileAsync(userId, veracityId);
            return Ok(response);
        }

        [HttpPost("overview/cards")]
        public async Task<ActionResult<ApiResponse<OverviewCardResponse>>> GetOverviewCards([FromBody] OverviewFilter filter)
        {
            var response = await _service.GetOverviewCardDataAsync(filter);
            return Ok(response);
        }

        [HttpGet("overview/filters")]
        public async Task<ActionResult<ApiResponse<List<OverviewCompanyServiceSiteFilterResult>>>> GetOverviewFilters()
        {
            var response = await _service.GetOverviewCompanyServiceSiteFilterAsync();
            return Ok(response);
        }

        [HttpGet("overview/financial-status")]
        public async Task<ActionResult<ApiResponse<List<WidgetFinancialStatusResponse>>>> GetFinancialStatus()
        {
            var response = await _service.GetOverviewFinancialStatusAsync();
            return Ok(response);
        }

        [HttpGet("widgets/training-status")]
        public async Task<ActionResult<ApiResponse<WidgetTrainingDataResponse>>> GetTrainingStatus([FromQuery] string? userId)
        {
            var response = await _service.GetWidgetForTrainingStatusAsync(userId);
            return Ok(response);
        }

        [HttpGet("widgets/upcoming-audit")]
        public async Task<ActionResult<ApiResponse<List<UpcomingAuditResponse>>>> GetUpcomingAudit(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var response = await _service.GetWidgetForUpcomingAuditAsync(startDate, endDate);
            return Ok(response);
        }

        [HttpGet("preferences")]
        public async Task<ActionResult<ApiResponse<PreferenceResponse>>> GetPreferences(
            [FromQuery] string objectType,
            [FromQuery] string objectName,
            [FromQuery] string pageName)
        {
            var response = await _service.GetPreferencesAsync(objectType, objectName, pageName);
            return Ok(response);
        }
    }
}
