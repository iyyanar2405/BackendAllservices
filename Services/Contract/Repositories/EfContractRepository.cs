using ContractService.Data;
using ContractService.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ContractService.Repositories
{
    public class EfContractRepository : IContractRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public EfContractRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<ContractListResponse>> GetContractListAsync(int pageNumber, int pageSize, string? companyId, string? contractType)
        {
            var rows = await _dbContext.Database
                .SqlQuery<ContractListResponse>($"EXEC Sp_GetContractList @companyId={companyId}, @contractType={contractType}, @pageSize={pageSize}, @pageNumber={pageNumber}")
                .ToListAsync();

            return rows;
        }

        public async Task<IReadOnlyList<ServiceDetailsResponse>> GetServiceListAsync()
        {
            var rows = await _dbContext.Database
                .SqlQueryRaw<ServiceDetailsResponse>("EXEC Sp_GetServiceList")
                .ToListAsync();

            return rows;
        }

        public async Task<IReadOnlyList<SiteDetailsResponse>> GetMasterSiteListAsync()
        {
            var rows = await _dbContext.Database
                .SqlQueryRaw<SiteDetailsResponse>("EXEC Sp_GetMasterSiteList")
                .ToListAsync();

            return rows;
        }

        public async Task<UserValidationResponse?> GetUserValidationAsync(string? userId, string? veracityId)
        {
            var rows = await _dbContext.Database
                .SqlQuery<UserValidationResponse>($"EXEC Sp_GetUserValidation @userId={userId}, @veracityId={veracityId}")
                .ToListAsync();

            return rows.FirstOrDefault();
        }

        public async Task<UserProfileDetailsResponse?> GetUserProfileAsync(string? userId, string? veracityId)
        {
            var rows = await _dbContext.Database
                .SqlQuery<UserProfileDetailsResponse>($"EXEC Sp_GetUserProfile @userId={userId}, @veracityId={veracityId}")
                .ToListAsync();

            var profile = rows.FirstOrDefault();
            if (profile == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(userId))
            {
                var accessRows = await _dbContext.Database
                    .SqlQuery<AccessRoleRow>($"SELECT r.RoleName FROM UserRoles ur JOIN Roles r ON ur.RoleId = r.RoleId WHERE ur.UserId = {userId}")
                    .ToListAsync();

                profile.AccessLevel = accessRows
                    .Select(row => new AccessRoleDetail
                    {
                        RoleName = row.RoleName,
                        RoleLevel = new List<int> { 1 }
                    })
                    .ToList();
            }

            return profile;
        }

        public async Task<OverviewCardResponse?> GetOverviewCardDataAsync(OverviewFilter filter)
        {
            var jsonRow = await _dbContext.Database
                .SqlQuery<JsonPayloadRow>($"EXEC Sp_GetOverviewCardData @filterCompanies={JsonSerializer.Serialize(filter.Companies)}, @filterSites={JsonSerializer.Serialize(filter.Sites)}, @filterServices={JsonSerializer.Serialize(filter.Services)}")
                .ToListAsync();

            var parsed = TryParseJsonResponse<OverviewCardResponse>(jsonRow.FirstOrDefault());
            if (parsed?.Data != null)
            {
                return parsed.Data;
            }

            var rows = await _dbContext.Database
                .SqlQuery<OverviewCardRow>($"EXEC Sp_GetOverviewCardData @filterCompanies={JsonSerializer.Serialize(filter.Companies)}, @filterSites={JsonSerializer.Serialize(filter.Sites)}, @filterServices={JsonSerializer.Serialize(filter.Services)}")
                .ToListAsync();

            if (!rows.Any())
            {
                return new OverviewCardResponse { Data = new List<OverviewServiceData>(), TotalItems = 0 };
            }

            var serviceGroups = rows
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
            var rows = await _dbContext.Database
                .SqlQueryRaw<OverviewCompanyServiceSiteFilterResult>("EXEC Sp_GetOverviewCompanyServiceSiteFilter")
                .ToListAsync();

            return rows;
        }

        public async Task<IReadOnlyList<WidgetFinancialStatusResponse>> GetOverviewFinancialStatusAsync()
        {
            var rows = await _dbContext.Database
                .SqlQueryRaw<WidgetFinancialStatusResponse>("EXEC Sp_GetOverviewFinancialStatus")
                .ToListAsync();

            return rows;
        }

        public async Task<WidgetTrainingDataResponse?> GetWidgetForTrainingStatusAsync(string? userId)
        {
            var jsonRow = await _dbContext.Database
                .SqlQuery<JsonPayloadRow>($"EXEC Sp_GetWidgetForTrainingStatus @userId={userId}")
                .ToListAsync();

            var parsed = TryParseJsonResponse<WidgetTrainingDataResponse>(jsonRow.FirstOrDefault());
            if (parsed?.Data != null)
            {
                return parsed.Data;
            }

            var items = await _dbContext.Database
                .SqlQuery<TrainingStatusItem>($"EXEC Sp_GetWidgetForTrainingStatus @userId={userId}")
                .ToListAsync();

            return new WidgetTrainingDataResponse { TrainingData = items.ToList() };
        }

        public async Task<IReadOnlyList<UpcomingAuditResponse>> GetWidgetForUpcomingAuditAsync(DateTime? startDate, DateTime? endDate)
        {
            var jsonRow = await _dbContext.Database
                .SqlQuery<JsonPayloadRow>($"EXEC Sp_GetWidgetForUpcomingAudit @startDate={startDate}, @endDate={endDate}")
                .ToListAsync();

            var parsed = TryParseJsonResponse<List<UpcomingAuditResponse>>(jsonRow.FirstOrDefault());
            if (parsed?.Data != null)
            {
                return parsed.Data;
            }

            var items = await _dbContext.Database
                .SqlQuery<UpcomingAuditResponse>($"EXEC Sp_GetWidgetForUpcomingAudit @startDate={startDate}, @endDate={endDate}")
                .ToListAsync();

            return items;
        }

        public async Task<PreferenceResponse?> GetPreferencesAsync(string objectType, string objectName, string pageName)
        {
            var rows = await _dbContext.Database
                .SqlQuery<PreferenceResponse>($"EXEC Sp_GetPreferences @objectType={objectType}, @objectName={objectName}, @pageName={pageName}")
                .ToListAsync();

            return rows.FirstOrDefault();
        }

        private static ApiResponse<T>? TryParseJsonResponse<T>(JsonPayloadRow? row)
        {
            if (row == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(row.JsonResponse))
            {
                return DeserializeApiResponse<T>(row.JsonResponse);
            }

            if (!string.IsNullOrWhiteSpace(row.Data))
            {
                var trimmed = row.Data.TrimStart();
                if (trimmed.StartsWith("{") || trimmed.StartsWith("["))
                {
                    var data = JsonSerializer.Deserialize<T>(row.Data, JsonOptions());
                    return new ApiResponse<T>
                    {
                        Data = data,
                        IsSuccess = true,
                        Message = string.Empty,
                        ErrorCode = string.Empty
                    };
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

        private sealed class JsonPayloadRow
        {
            public string? JsonResponse { get; set; }
            public string? Data { get; set; }
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
