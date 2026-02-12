using Dapper;
using ScheduleService.Data;
using ScheduleService.Models;
using System.Data;
using System.Text.Json;

namespace ScheduleService.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly DapperContext _context;

        public ScheduleRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<AuditScheduleResponse>>> GetAuditSchedulesAsync(CalendarScheduleFilterInput filter)
        {
            using var connection = _context.CreateConnection();
            var parameters = new
            {
                userId = (int?)null,
                calendarScheduleFilter = filter
            };

            var row = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "Sp_GetAuditSchedules",
                new { Parameters = JsonSerializer.Serialize(parameters) },
                commandType: CommandType.StoredProcedure);

            return ParseJsonResponse<List<AuditScheduleResponse>>(row, "Audit schedules not available");
        }

        public async Task<ApiResponse<CalendarResponse>> GetScheduleCalendarInviteAsync(bool isAddToCalender, int siteAuditId)
        {
            using var connection = _context.CreateConnection();
            var parameters = new
            {
                userId = (int?)null,
                isAddToCalender,
                siteAuditId
            };

            var row = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "Sp_GetScheduleCalendarInvite",
                new { Parameters = JsonSerializer.Serialize(parameters) },
                commandType: CommandType.StoredProcedure);

            return ParseJsonResponse<CalendarResponse>(row, "Calendar invite not available");
        }

        private static ApiResponse<T> ParseJsonResponse<T>(object? row, string fallbackMessage)
        {
            if (row is IDictionary<string, object> dict)
            {
                if (dict.TryGetValue("JsonResponse", out var jsonValue) && jsonValue != null)
                {
                    return DeserializeApiResponse<T>(jsonValue.ToString(), fallbackMessage);
                }
            }

            return new ApiResponse<T>
            {
                Data = default,
                IsSuccess = false,
                Message = fallbackMessage,
                ErrorCode = "NOT_IMPLEMENTED"
            };
        }

        private static ApiResponse<T> DeserializeApiResponse<T>(string? json, string fallbackMessage)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new ApiResponse<T>
                {
                    Data = default,
                    IsSuccess = false,
                    Message = fallbackMessage,
                    ErrorCode = "EMPTY_RESPONSE"
                };
            }

            try
            {
                var response = JsonSerializer.Deserialize<ApiResponse<T>>(json, JsonOptions());
                if (response != null)
                {
                    return response;
                }
            }
            catch
            {
            }

            return new ApiResponse<T>
            {
                Data = default,
                IsSuccess = false,
                Message = fallbackMessage,
                ErrorCode = "PARSE_ERROR"
            };
        }

        private static JsonSerializerOptions JsonOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }
    }
}
