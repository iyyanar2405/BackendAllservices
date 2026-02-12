using AuditService.Models;
using AuditService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuditService.Controllers
{
    [ApiController]
    [Route("api/audits")]
    public class AuditsController : ControllerBase
    {
        private readonly IAuditService _service;

        public AuditsController(IAuditService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<AuditListResponse>>>> GetAudits()
        {
            var response = await _service.GetAuditListAsync();
            return Ok(response);
        }

        [HttpGet("{auditId:int}")]
        public async Task<ActionResult<ApiResponse<AuditDetailResponse>>> GetAuditDetails(int auditId)
        {
            var response = await _service.GetAuditDetailsAsync(auditId);
            return Ok(response);
        }

        [HttpGet("{auditId:int}/findings")]
        public async Task<ActionResult<ApiResponse<List<AuditFindingListResponse>>>> GetAuditFindings(int auditId)
        {
            var response = await _service.GetAuditFindingsAsync(auditId);
            return Ok(response);
        }

        [HttpGet("{auditId:int}/sites")]
        public async Task<ActionResult<ApiResponse<List<AuditSiteResponse>>>> GetAuditSites(int auditId)
        {
            var response = await _service.GetAuditSitesAsync(auditId);
            return Ok(response);
        }

        [HttpGet("{auditId:int}/subaudits")]
        public async Task<ActionResult<ApiResponse<List<SubAuditResponse>>>> GetSubAudits(int auditId)
        {
            var response = await _service.GetSubAuditsAsync(auditId);
            return Ok(response);
        }

        [HttpGet("days/grid")]
        public async Task<ActionResult<ApiResponse<AuditDaysGridResponse>>> GetAuditDaysGrid(
            [FromQuery] string startDate,
            [FromQuery] string endDate,
            [FromQuery] List<int>? companies,
            [FromQuery] List<string>? services,
            [FromQuery] List<int>? sites)
        {
            var response = await _service.GetAuditDaysGridAsync(
                startDate,
                endDate,
                companies ?? new List<int>(),
                services ?? new List<string>(),
                sites ?? new List<int>());

            return Ok(response);
        }

        [HttpPost("days/by-service")]
        public async Task<ActionResult<ApiResponse<AuditDaysByServiceResponse>>> GetAuditDaysByService([FromBody] AuditDaysFilter filters)
        {
            var response = await _service.GetAuditDaysByServiceAsync(filters);
            return Ok(response);
        }

        [HttpPost("days/by-month-service")]
        public async Task<ActionResult<ApiResponse<AuditDaysByMonthAndServiceResponse>>> GetAuditDaysByMonthAndService([FromBody] AuditDaysByMonthFilter filters)
        {
            var response = await _service.GetAuditDaysByMonthAndServiceAsync(filters);
            return Ok(response);
        }
    }
}
