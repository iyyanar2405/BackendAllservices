using ScheduleService.Models;

namespace ScheduleService.Services
{
    public interface IScheduleService
    {
        Task<ApiResponse<List<AuditScheduleResponse>>> GetAuditSchedulesAsync(CalendarScheduleFilterInput filter);
        Task<ApiResponse<CalendarResponse>> GetScheduleCalendarInviteAsync(bool isAddToCalender, int siteAuditId);
    }
}
