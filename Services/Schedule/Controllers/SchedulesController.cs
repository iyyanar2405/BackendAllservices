using Microsoft.AspNetCore.Mvc;
using ScheduleService.Models;
using ScheduleService.Services;

namespace ScheduleService.Controllers
{
    [ApiController]
    [Route("api/schedules")]
    public class SchedulesController : ControllerBase
    {
        private readonly IScheduleService _service;

        public SchedulesController(IScheduleService service)
        {
            _service = service;
        }

        [HttpPost("search")]
        public async Task<ActionResult<ApiResponse<List<AuditScheduleResponse>>>> GetSchedules([FromBody] CalendarScheduleFilterInput filter)
        {
            var response = await _service.GetAuditSchedulesAsync(filter);
            return Ok(response);
        }

        [HttpGet("calendar-invite")]
        public async Task<ActionResult<ApiResponse<CalendarResponse>>> GetCalendarInvite(
            [FromQuery] bool isAddToCalender,
            [FromQuery] int siteAuditId)
        {
            var response = await _service.GetScheduleCalendarInviteAsync(isAddToCalender, siteAuditId);
            return Ok(response);
        }
    }
}
