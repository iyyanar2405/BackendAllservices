using ScheduleService.Models;
using ScheduleService.Services;

namespace ScheduleService.GraphQL.Queries
{
    public class Query
    {
        private readonly IScheduleService _service;

        public Query(IScheduleService service)
        {
            _service = service;
        }

        [GraphQLName("viewAuditSchedules")]
        public Task<ApiResponse<List<AuditScheduleResponse>>> ViewAuditSchedules(CalendarScheduleFilterInput calendarScheduleFilter)
        {
            return _service.GetAuditSchedulesAsync(calendarScheduleFilter);
        }

        [GraphQLName("addToCalender")]
        public Task<ApiResponse<CalendarResponse>> AddToCalender(bool isAddToCalender, int siteAuditId)
        {
            return _service.GetScheduleCalendarInviteAsync(isAddToCalender, siteAuditId);
        }
    }
}
