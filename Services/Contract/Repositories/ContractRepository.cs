using ContractService.Data;
using ContractService.Models;
using Dapper;
using System.Data;
using System.Text.Json;

namespace ContractService.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private readonly DapperContext _context;

        public ContractRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<ContractListResponse>> GetContractListAsync(int pageNumber, int pageSize, string? companyId, string? contractType)
        {
            using var connection = _context.CreateConnection();
            var rows = await connection.QueryAsync<ContractListResponse>(
                "Sp_GetContractList",
                new { companyId, contractType, pageSize, pageNumber },
                commandType: CommandType.StoredProcedure);

            return rows.ToList();
        }

        public async Task<IReadOnlyList<ServiceDetailsResponse>> GetServiceListAsync()
        {
            using var connection = _context.CreateConnection();
            var rows = await connection.QueryAsync<ServiceDetailsResponse>(
                "Sp_GetServiceList",
                commandType: CommandType.StoredProcedure);

            return rows.ToList();
        }

        public async Task<IReadOnlyList<SiteDetailsResponse>> GetMasterSiteListAsync()
        {
            using var connection = _context.CreateConnection();
            var rows = await connection.QueryAsync<SiteDetailsResponse>(
                "Sp_GetMasterSiteList",
                commandType: CommandType.StoredProcedure);

            return rows.ToList();
        }

        public async Task<UserValidationResponse?> GetUserValidationAsync(string? userId, string? veracityId)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<UserValidationResponse>(
                "Sp_GetUserValidation",
                new { userId, veracityId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<UserProfileDetailsResponse?> GetUserProfileAsync(string? userId, string? veracityId)
        {
            using var connection = _context.CreateConnection();
            var profile = await connection.QueryFirstOrDefaultAsync<UserProfileDetailsResponse>(
                "Sp_GetUserProfile",
                new { userId, veracityId },
                commandType: CommandType.StoredProcedure);

            if (profile == null)
            {
                return null;
            }

            var accessRows = await connection.QueryAsync<AccessRoleRow>(
                "SELECT r.RoleName FROM UserRoles ur JOIN Roles r ON ur.RoleId = r.RoleId WHERE ur.UserId = @userId",
                new { userId });

            profile.AccessLevel = accessRows
                .Select(row => new AccessRoleDetail
                {
                    RoleName = row.RoleName,
                    RoleLevel = new List<int> { 1 }
                })
                .ToList();

            return profile;
        }

        public async Task<OverviewCardResponse?> GetOverviewCardDataAsync(OverviewFilter filter)
        {
            using var connection = _context.CreateConnection();
            var row = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "Sp_GetOverviewCardData",
                new
                {
                    filterCompanies = JsonSerializer.Serialize(filter.Companies),
                    filterSites = JsonSerializer.Serialize(filter.Sites),
                    filterServices = JsonSerializer.Serialize(filter.Services)
                },
                commandType: CommandType.StoredProcedure);

            var parsed = TryParseJsonResponse<OverviewCardResponse>(row);
            if (parsed?.Data != null)
            {
                return parsed.Data;
            }

            var rows = await connection.QueryAsync<OverviewCardRow>(
                "Sp_GetOverviewCardData",
                new
                {
                    filterCompanies = JsonSerializer.Serialize(filter.Companies),
                    filterSites = JsonSerializer.Serialize(filter.Sites),
                    filterServices = JsonSerializer.Serialize(filter.Services)
                },
                commandType: CommandType.StoredProcedure);

            var rowList = rows.ToList();
            if (!rowList.Any())
            {
                return new OverviewCardResponse { Data = new List<OverviewServiceData>(), TotalItems = 0 };
            }

            var serviceGroups = rowList
                .GroupBy(r => new { r.ServiceId, r.ServiceName })
                .Select(serviceGroup => new OverviewServiceData
                {
                    ServiceId = serviceGroup.Key.ServiceId,
                    ServiceName = serviceGroup.Key.ServiceName,
                    YearData = serviceGroup
                        .GroupBy(r => r.Year)
                        .Select(yearGroup => new OverviewYearData
                        {
                            Year = yearGroup.Key,
                            Values = yearGroup.Select(v => new OverviewValueData
                            {
                                Count = v.Count,
                                Seq = v.Seq,
                                StatusValue = v.StatusValue,
                                TotalCount = v.TotalCount
                            }).ToList()
                        }).ToList()
                }).ToList();

            return new OverviewCardResponse
            {
                Data = serviceGroups,
                TotalItems = serviceGroups.Count
            };
        }

        public async Task<IReadOnlyList<OverviewCompanyServiceSiteFilterResult>> GetOverviewCompanyServiceSiteFilterAsync()
        {
            using var connection = _context.CreateConnection();
            var rows = await connection.QueryAsync<OverviewCompanyServiceSiteFilterResult>(
                "Sp_GetOverviewCompanyServiceSiteFilter",
                commandType: CommandType.StoredProcedure);

            return rows.ToList();
        }

        public async Task<IReadOnlyList<WidgetFinancialStatusResponse>> GetOverviewFinancialStatusAsync()
        {
            using var connection = _context.CreateConnection();
            var rows = await connection.QueryAsync<WidgetFinancialStatusResponse>(
                "Sp_GetOverviewFinancialStatus",
                commandType: CommandType.StoredProcedure);

            return rows.ToList();
        }

        public async Task<WidgetTrainingDataResponse?> GetWidgetForTrainingStatusAsync(string? userId)
        {
            using var connection = _context.CreateConnection();
            var row = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "Sp_GetWidgetForTrainingStatus",
                new { userId },
                commandType: CommandType.StoredProcedure);

            var parsed = TryParseJsonResponse<WidgetTrainingDataResponse>(row);
            if (parsed?.Data != null)
            {
                return parsed.Data;
            }

            var items = await connection.QueryAsync<TrainingStatusItem>(
                "Sp_GetWidgetForTrainingStatus",
                new { userId },
                commandType: CommandType.StoredProcedure);

            return new WidgetTrainingDataResponse { TrainingData = items.ToList() };
        }

        public async Task<IReadOnlyList<UpcomingAuditResponse>> GetWidgetForUpcomingAuditAsync(DateTime? startDate, DateTime? endDate)
        {
            using var connection = _context.CreateConnection();
            var row = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "Sp_GetWidgetForUpcomingAudit",
                new { startDate, endDate },
                commandType: CommandType.StoredProcedure);

            var parsed = TryParseJsonResponse<List<UpcomingAuditResponse>>(row);
            if (parsed?.Data != null)
            {
                return parsed.Data;
            }

            var items = await connection.QueryAsync<UpcomingAuditResponse>(
                "Sp_GetWidgetForUpcomingAudit",
                new { startDate, endDate },
                commandType: CommandType.StoredProcedure);

            return items.ToList();
        }

        public async Task<PreferenceResponse?> GetPreferencesAsync(string objectType, string objectName, string pageName)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<PreferenceResponse>(
                "Sp_GetPreferences",
                new { objectType, objectName, pageName },
                commandType: CommandType.StoredProcedure);
        }

        private static ApiResponse<T>? TryParseJsonResponse<T>(object? row)
        {
            if (row is IDictionary<string, object> dict)
            {
                if (dict.TryGetValue("JsonResponse", out var jsonValue) && jsonValue != null)
                {
                    return DeserializeApiResponse<T>(jsonValue.ToString());
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
                            Message = string.Empty,
                            ErrorCode = string.Empty
                        };
                    }
                }
            }

            return null;
        }

        private static ApiResponse<T>? DeserializeApiResponse<T>(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            return JsonSerializer.Deserialize<ApiResponse<T>>(json, JsonOptions());
        }

        private static JsonSerializerOptions JsonOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        private sealed class OverviewCardRow
        {
            public string? ServiceId { get; set; }
            public string? ServiceName { get; set; }
            public int Year { get; set; }
            public int Count { get; set; }
            public int Seq { get; set; }
            public string? StatusValue { get; set; }
            public int TotalCount { get; set; }
        }

        private sealed class AccessRoleRow
        {
            public string? RoleName { get; set; }
        }
    }
}
