using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ScheduleService.Data;
using ScheduleService.Models;

namespace ScheduleService.Repositories
{
    public class EfScheduleRepository : IScheduleRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public EfScheduleRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApiResponse<List<AuditScheduleResponse>>> GetAuditSchedulesAsync(CalendarScheduleFilterInput filter)
        {
            var parameters = new
            {
                userId = (int?)null,
                calendarScheduleFilter = filter
            };

            var json = await ExecuteJsonResponseAsync("Sp_GetAuditSchedules", parameters);
            return JsonResponseParser.ParseFromJson<List<AuditScheduleResponse>>(json, "Audit schedules not available");
        }

        public async Task<ApiResponse<CalendarResponse>> GetScheduleCalendarInviteAsync(bool isAddToCalender, int siteAuditId)
        {
            var parameters = new
            {
                userId = (int?)null,
                isAddToCalender,
                siteAuditId
            };

            var json = await ExecuteJsonResponseAsync("Sp_GetScheduleCalendarInvite", parameters);
            return JsonResponseParser.ParseFromJson<CalendarResponse>(json, "Calendar invite not available");
        }

        private async Task<string?> ExecuteJsonResponseAsync(string storedProcedure, object parameters)
        {
            var connection = _dbContext.Database.GetDbConnection();
            await using var command = connection.CreateCommand();
            command.CommandText = storedProcedure;
            command.CommandType = CommandType.StoredProcedure;

            var jsonParameters = JsonSerializer.Serialize(parameters);
            var dbParameter = command.CreateParameter();
            dbParameter.ParameterName = "@Parameters";
            dbParameter.DbType = DbType.String;
            dbParameter.Value = jsonParameters;
            command.Parameters.Add(dbParameter);

            var shouldClose = connection.State != ConnectionState.Open;
            if (shouldClose)
            {
                await connection.OpenAsync();
            }

            try
            {
                await using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var ordinal = TryGetOrdinal(reader, "JsonResponse");
                    if (ordinal >= 0 && !reader.IsDBNull(ordinal))
                    {
                        return reader.GetString(ordinal);
                    }

                    if (reader.FieldCount > 0 && !reader.IsDBNull(0))
                    {
                        return reader.GetValue(0)?.ToString();
                    }
                }
            }
            finally
            {
                if (shouldClose)
                {
                    await connection.CloseAsync();
                }
            }

            return null;
        }

        private static int TryGetOrdinal(IDataRecord record, string name)
        {
            for (var i = 0; i < record.FieldCount; i++)
            {
                if (string.Equals(record.GetName(i), name, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
