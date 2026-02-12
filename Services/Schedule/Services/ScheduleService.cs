using ScheduleService.Models;
using ScheduleService.Repositories;

namespace ScheduleService.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _repository;
        private readonly ILogger<ScheduleService> _logger;

        public ScheduleService(IScheduleRepository repository, ILogger<ScheduleService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ApiResponse<List<AuditScheduleResponse>>> GetAuditSchedulesAsync(CalendarScheduleFilterInput filter)
        {
            try
            {
                return await _repository.GetAuditSchedulesAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load audit schedules");
                return new ApiResponse<List<AuditScheduleResponse>>
                {
                    Data = default,
                    IsSuccess = false,
                    Message = "Failed to load audit schedules",
                    ErrorCode = "ERR_SCHEDULE"
                };
            }
        }

        public async Task<ApiResponse<CalendarResponse>> GetScheduleCalendarInviteAsync(bool isAddToCalender, int siteAuditId)
        {
            try
            {
                return await _repository.GetScheduleCalendarInviteAsync(isAddToCalender, siteAuditId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load calendar invite");
                return new ApiResponse<CalendarResponse>
                {
                    Data = default,
                    IsSuccess = false,
                    Message = "Failed to load calendar invite",
                    ErrorCode = "ERR_SCHEDULE"
                };
            }
        }
    }
}
