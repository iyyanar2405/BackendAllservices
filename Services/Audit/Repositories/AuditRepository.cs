using AuditService.Data;
using AuditService.Models;
using Dapper;
using System.Data;
using System.Text.Json;

namespace AuditService.Repositories
{
    public class AuditRepository : IAuditRepository
    {
        private readonly DapperContext _context;

        public AuditRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<AuditListResponse>> GetAuditListAsync()
        {
            using var connection = _context.CreateConnection();
            var audits = (await connection.QueryAsync<AuditListRow>(
                "SELECT AuditId, CompanyId, Status, StartDate, EndDate, LeadAuditor, Type FROM Audits"))
                .ToList();

            var siteRows = await connection.QueryAsync<AuditSiteIdRow>(
                "SELECT AuditId, SiteId FROM AuditSiteAudits");
            var serviceRows = await connection.QueryAsync<AuditServiceIdRow>(
                "SELECT AuditId, ServiceId FROM AuditServices");

            var siteMap = siteRows
                .GroupBy(row => row.AuditId)
                .ToDictionary(g => g.Key, g => g.Select(r => r.SiteId).Distinct().ToList());

            var serviceMap = serviceRows
                .GroupBy(row => row.AuditId)
                .ToDictionary(g => g.Key, g => g.Select(r => r.ServiceId).Distinct().ToList());

            return audits.Select(row => new AuditListResponse
            {
                AuditId = row.AuditId,
                CompanyId = row.CompanyId ?? 0,
                Status = row.Status,
                StartDate = row.StartDate,
                EndDate = row.EndDate,
                LeadAuditor = row.LeadAuditor,
                Type = row.Type,
                Sites = siteMap.TryGetValue(row.AuditId, out var sites) ? sites : new List<int>(),
                Services = serviceMap.TryGetValue(row.AuditId, out var services) ? services : new List<int>()
            }).ToList();
        }

        public async Task<AuditDetailResponse?> GetAuditDetailsAsync(int auditId)
        {
            using var connection = _context.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<AuditDetailRow>(
                "Sp_GetAuditDetails",
                new { auditId },
                commandType: CommandType.StoredProcedure);

            if (result == null || result.AuditId == null)
            {
                return null;
            }

            var services = await GetAuditServiceNamesAsync(connection, auditId);
            var auditorTeam = string.IsNullOrWhiteSpace(result.LeadAuditor)
                ? new List<string>()
                : new List<string> { result.LeadAuditor };

            return new AuditDetailResponse
            {
                AuditId = result.AuditId.Value,
                EndDate = result.EndDate,
                LeadAuditor = result.LeadAuditor,
                SiteAddress = result.SiteAddress,
                SiteName = result.SiteName,
                StartDate = result.StartDate,
                Status = result.Status,
                Services = services,
                AuditorTeam = auditorTeam
            };
        }

        public async Task<IReadOnlyList<AuditFindingListResponse>> GetAuditFindingsAsync(int auditId)
        {
            using var connection = _context.CreateConnection();
            var rows = await connection.QueryAsync<AuditFindingRow>(
                "Sp_GetAuditFindingList",
                new { auditId },
                commandType: CommandType.StoredProcedure);

            return rows.Select(row => new AuditFindingListResponse
            {
                AcceptedDate = row.AcceptedDate,
                AuditId = row.AuditId,
                Category = row.Category,
                CompanyId = row.CompanyId,
                ClosedDate = row.ClosedDate,
                DueDate = row.DueDate,
                FindingNumber = row.FindingNumber,
                FindingsId = row.FindingsId,
                OpenDate = row.OpenDate,
                Services = SplitList(row.Services),
                SiteId = row.SiteId,
                Status = row.Status,
                Title = row.Title
            }).ToList();
        }

        public async Task<IReadOnlyList<AuditSiteResponse>> GetAuditSitesAsync(int auditId)
        {
            using var connection = _context.CreateConnection();
            var rows = await connection.QueryAsync<AuditSiteResponse>(
                "Sp_GetAuditSites",
                new { auditId },
                commandType: CommandType.StoredProcedure);

            return rows.ToList();
        }

        public async Task<IReadOnlyList<SubAuditResponse>> GetSubAuditsAsync(int auditId)
        {
            using var connection = _context.CreateConnection();
            var rows = await connection.QueryAsync<SubAuditRow>(
                "Sp_GetSubAudits",
                new { auditId },
                commandType: CommandType.StoredProcedure);

            return rows.Select(row => new SubAuditResponse
            {
                AuditId = row.AuditId,
                Sites = SplitIntList(row.Sites),
                Services = SplitIntList(row.Services),
                Status = row.Status,
                StartDate = row.StartDate,
                EndDate = row.EndDate,
                AuditorTeam = SplitList(row.AuditorTeam)
            }).ToList();
        }

        public async Task<ApiResponse<AuditDaysGridResponse>> GetAuditDaysGridAsync(string startDate, string endDate, List<int> companies, List<string> services, List<int> sites)
        {
            using var connection = _context.CreateConnection();
            var row = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "Sp_GetAuditDaysGrid",
                new
                {
                    startDate,
                    endDate,
                    companies = JsonSerializer.Serialize(companies),
                    services = JsonSerializer.Serialize(services),
                    sites = JsonSerializer.Serialize(sites)
                },
                commandType: CommandType.StoredProcedure);

            return ParseJsonResponse<AuditDaysGridResponse>(row, "Audit days grid not available");
        }

        public async Task<ApiResponse<AuditDaysByServiceResponse>> GetAuditDaysByServiceAsync(AuditDaysFilter filters)
        {
            using var connection = _context.CreateConnection();
            var row = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "Sp_GetAuditDaysByService",
                new
                {
                    startDate = filters.StartDate,
                    endDate = filters.EndDate,
                    companies = JsonSerializer.Serialize(filters.Companies),
                    services = JsonSerializer.Serialize(filters.Services),
                    sites = JsonSerializer.Serialize(filters.Sites)
                },
                commandType: CommandType.StoredProcedure);

            return ParseJsonResponse<AuditDaysByServiceResponse>(row, "Audit days by service not available");
        }

        public async Task<ApiResponse<AuditDaysByMonthAndServiceResponse>> GetAuditDaysByMonthAndServiceAsync(AuditDaysByMonthFilter filters)
        {
            using var connection = _context.CreateConnection();
            var row = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "Sp_GetAuditDaysByMonthAndService",
                new
                {
                    startDate = filters.StartDate,
                    endDate = filters.EndDate,
                    companyFilter = JsonSerializer.Serialize(filters.CompanyFilter),
                    serviceFilter = JsonSerializer.Serialize(filters.ServiceFilter),
                    siteFilter = JsonSerializer.Serialize(filters.SiteFilter)
                },
                commandType: CommandType.StoredProcedure);

            return ParseJsonResponse<AuditDaysByMonthAndServiceResponse>(row, "Audit days by month and service not available");
        }

        private static List<string> SplitList(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return new List<string>();
            }

            return value
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();
        }

        private static List<int> SplitIntList(string? value)
        {
            return SplitList(value)
                .Select(item => int.TryParse(item, out var id) ? id : 0)
                .Where(id => id > 0)
                .ToList();
        }

        private async Task<List<string>> GetAuditServiceNamesAsync(IDbConnection connection, int auditId)
        {
            var rows = await connection.QueryAsync<string>(
                "SELECT DISTINCT s.ServiceName FROM AuditServices a JOIN Services s ON a.ServiceId = s.ServiceId WHERE a.AuditId = @auditId",
                new { auditId });

            return rows.ToList();
        }

        private static ApiResponse<T> ParseJsonResponse<T>(object? row, string fallbackMessage)
        {
            if (row is IDictionary<string, object> dict)
            {
                if (dict.TryGetValue("JsonResponse", out var jsonValue) && jsonValue != null)
                {
                    return DeserializeApiResponse<T>(jsonValue.ToString(), fallbackMessage);
                }

                if (dict.TryGetValue("data", out var dataValue) && dataValue != null)
                {
                    var dataJson = dataValue.ToString();
                    if (!string.IsNullOrWhiteSpace(dataJson) && dataJson.TrimStart().StartsWith("{"))
                    {
                        var data = JsonSerializer.Deserialize<T>(dataJson, JsonOptions());
                        return new ApiResponse<T>
                        {
                            Data = data,
                            IsSuccess = true,
                            Message = dict.TryGetValue("message", out var message) ? message?.ToString() : "",
                            ErrorCode = dict.TryGetValue("errorCode", out var errorCode) ? errorCode?.ToString() : ""
                        };
                    }
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

        private sealed class AuditListRow
        {
            public int AuditId { get; set; }
            public int? CompanyId { get; set; }
            public string? Status { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string? LeadAuditor { get; set; }
            public string? Type { get; set; }
        }

        private sealed class AuditSiteIdRow
        {
            public int AuditId { get; set; }
            public int SiteId { get; set; }
        }

        private sealed class AuditServiceIdRow
        {
            public int AuditId { get; set; }
            public int ServiceId { get; set; }
        }

        private sealed class AuditDetailRow
        {
            public int? AuditId { get; set; }
            public string? EndDate { get; set; }
            public string? LeadAuditor { get; set; }
            public string? SiteAddress { get; set; }
            public string? SiteName { get; set; }
            public string? StartDate { get; set; }
            public string? Status { get; set; }
        }

        private sealed class AuditFindingRow
        {
            public string? AcceptedDate { get; set; }
            public int AuditId { get; set; }
            public string? Category { get; set; }
            public int CompanyId { get; set; }
            public string? ClosedDate { get; set; }
            public string? DueDate { get; set; }
            public string? FindingNumber { get; set; }
            public int FindingsId { get; set; }
            public string? OpenDate { get; set; }
            public string? Services { get; set; }
            public int SiteId { get; set; }
            public string? Status { get; set; }
            public string? Title { get; set; }
        }

        private sealed class SubAuditRow
        {
            public int AuditId { get; set; }
            public string? Sites { get; set; }
            public string? Services { get; set; }
            public string? Status { get; set; }
            public string? StartDate { get; set; }
            public string? EndDate { get; set; }
            public string? AuditorTeam { get; set; }
        }
    }
}
