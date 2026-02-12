using ScheduleService.Models;

namespace ScheduleService.Repositories
{
    public interface IScheduleRepository
    {
        Task<ApiResponse<List<AuditScheduleResponse>>> GetAuditSchedulesAsync(CalendarScheduleFilterInput filter);
        Task<ApiResponse<CalendarResponse>> GetScheduleCalendarInviteAsync(bool isAddToCalender, int siteAuditId);
    }
}
