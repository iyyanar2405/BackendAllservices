using AuditService.Data;
using AuditService.Data.Entities;
using AuditService.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;
using System.Text.Json;

namespace AuditService.Repositories
{
    public class AuditEfRepository : IAuditRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public AuditEfRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<AuditListResponse>> GetAuditListAsync()
        {
            var audits = await _dbContext.Audits.AsNoTracking().ToListAsync();

            var auditSiteIds = await _dbContext.AuditSiteAudits.AsNoTracking()
                .Select(item => new { item.AuditId, item.SiteId })
                .ToListAsync();

            var auditSites = await _dbContext.AuditSites.AsNoTracking()
                .Select(item => new { item.AuditId, item.SiteId })
                .ToListAsync();

            var auditServices = await _dbContext.AuditServices.AsNoTracking()
                .Select(item => new { item.AuditId, item.ServiceId })
                .ToListAsync();

            var siteMap = auditSiteIds
                .Concat(auditSites)
                .GroupBy(item => item.AuditId)
                .ToDictionary(group => group.Key, group => group.Select(item => item.SiteId).Distinct().ToList());

            var serviceMap = auditServices
                .GroupBy(item => item.AuditId)
                .ToDictionary(group => group.Key, group => group.Select(item => item.ServiceId).Distinct().ToList());

            return audits.Select(audit => new AuditListResponse
            {
                AuditId = audit.AuditId,
                CompanyId = audit.CompanyId ?? 0,
                Status = audit.Status,
                StartDate = audit.StartDate,
                EndDate = audit.EndDate,
                LeadAuditor = audit.LeadAuditor,
                Type = audit.Type,
                Sites = siteMap.TryGetValue(audit.AuditId, out var sites) ? sites : new List<int>(),
                Services = serviceMap.TryGetValue(audit.AuditId, out var services) ? services : new List<int>()
            }).ToList();
        }

        public async Task<AuditDetailResponse?> GetAuditDetailsAsync(int auditId)
        {
            var audit = await _dbContext.Audits.AsNoTracking()
                .FirstOrDefaultAsync(item => item.AuditId == auditId);

            if (audit == null)
            {
                return null;
            }

            var siteDetail = await (from auditSite in _dbContext.AuditSites.AsNoTracking()
                join site in _dbContext.Sites.AsNoTracking() on auditSite.SiteId equals site.SiteId
                where auditSite.AuditId == auditId
                select new { site.SiteName, site.Address, site.CityId, site.CountryId, site.PostalCode })
                .FirstOrDefaultAsync();

            var cityName = await GetCityNameAsync(siteDetail?.CityId);
            var countryName = await GetCountryNameAsync(siteDetail?.CountryId);

            var siteAddress = BuildSiteAddress(siteDetail?.Address, cityName, siteDetail?.PostalCode, countryName);

            var services = await (from auditService in _dbContext.AuditServices.AsNoTracking()
                join service in _dbContext.Services.AsNoTracking() on auditService.ServiceId equals service.ServiceId
                where auditService.AuditId == auditId
                select service.ServiceName).Distinct().ToListAsync();

            var auditorTeam = string.IsNullOrWhiteSpace(audit.LeadAuditor)
                ? new List<string>()
                : new List<string> { audit.LeadAuditor };

            return new AuditDetailResponse
            {
                AuditId = audit.AuditId,
                EndDate = audit.EndDate?.ToString("yyyy-MM-dd"),
                LeadAuditor = audit.LeadAuditor,
                SiteAddress = siteAddress,
                SiteName = siteDetail?.SiteName,
                StartDate = audit.StartDate?.ToString("yyyy-MM-dd"),
                Status = audit.Status,
                Services = services,
                AuditorTeam = auditorTeam
            };
        }

        public async Task<IReadOnlyList<AuditFindingListResponse>> GetAuditFindingsAsync(int auditId)
        {
            var findings = await _dbContext.Findings.AsNoTracking()
                .Where(item => item.AuditId == auditId)
                .ToListAsync();

            var categoryLookup = await _dbContext.FindingCategories.AsNoTracking()
                .ToDictionaryAsync(item => item.FindingCategoryId, item => item.CategoryName);

            var statusLookup = await _dbContext.FindingStatuses.AsNoTracking()
                .ToDictionaryAsync(item => item.FindingStatusId, item => item.StatusName);

            return findings.Select(item => new AuditFindingListResponse
            {
                AcceptedDate = null,
                AuditId = item.AuditId,
                Category = item.FindingCategoryId.HasValue && categoryLookup.TryGetValue(item.FindingCategoryId.Value, out var category)
                    ? category
                    : null,
                CompanyId = 0,
                ClosedDate = item.ClosedDate?.ToString("yyyy-MM-dd"),
                DueDate = item.DueDate?.ToString("yyyy-MM-dd"),
                FindingNumber = item.FindingNumber,
                FindingsId = item.FindingId,
                OpenDate = item.IdentifiedDate.ToString("yyyy-MM-dd"),
                Services = new List<string>(),
                SiteId = item.SiteId ?? 0,
                Status = statusLookup.TryGetValue(item.FindingStatusId, out var status) ? status : null,
                Title = item.Title
            }).ToList();
        }

        public async Task<IReadOnlyList<AuditSiteResponse>> GetAuditSitesAsync(int auditId)
        {
            var siteDetails = await (from auditSite in _dbContext.AuditSites.AsNoTracking()
                join site in _dbContext.Sites.AsNoTracking() on auditSite.SiteId equals site.SiteId
                where auditSite.AuditId == auditId
                select new { site.SiteName, site.Address, site.CityId, site.CountryId, site.PostalCode })
                .ToListAsync();

            var cityLookup = await _dbContext.Cities.AsNoTracking().ToDictionaryAsync(item => item.CityId, item => item.CityName);
            var countryLookup = await _dbContext.Countries.AsNoTracking().ToDictionaryAsync(item => item.CountryId, item => item.CountryName);

            return siteDetails.Select(site => new AuditSiteResponse
            {
                SiteName = site.SiteName,
                AddressLine = site.Address,
                City = site.CityId.HasValue && cityLookup.TryGetValue(site.CityId.Value, out var city) ? city : null,
                Country = site.CountryId.HasValue && countryLookup.TryGetValue(site.CountryId.Value, out var country) ? country : null,
                PostCode = site.PostalCode
            }).ToList();
        }

        public async Task<IReadOnlyList<SubAuditResponse>> GetSubAuditsAsync(int auditId)
        {
            var audit = await _dbContext.Audits.AsNoTracking().FirstOrDefaultAsync(item => item.AuditId == auditId);
            if (audit == null)
            {
                return new List<SubAuditResponse>();
            }

            var sites = await _dbContext.AuditSiteAudits.AsNoTracking()
                .Where(item => item.AuditId == auditId)
                .Select(item => item.SiteId)
                .ToListAsync();

            var siteFallback = await _dbContext.AuditSites.AsNoTracking()
                .Where(item => item.AuditId == auditId)
                .Select(item => item.SiteId)
                .ToListAsync();

            var services = await _dbContext.AuditServices.AsNoTracking()
                .Where(item => item.AuditId == auditId)
                .Select(item => item.ServiceId)
                .ToListAsync();

            var auditorTeam = string.IsNullOrWhiteSpace(audit.LeadAuditor)
                ? new List<string>()
                : new List<string> { audit.LeadAuditor };

            return new List<SubAuditResponse>
            {
                new SubAuditResponse
                {
                    AuditId = audit.AuditId,
                    Sites = sites.Concat(siteFallback).Distinct().ToList(),
                    Services = services.Distinct().ToList(),
                    Status = audit.Status,
                    StartDate = audit.StartDate?.ToString("yyyy-MM-dd"),
                    EndDate = audit.EndDate?.ToString("yyyy-MM-dd"),
                    AuditorTeam = auditorTeam
                }
            };
        }

        public async Task<ApiResponse<AuditDaysGridResponse>> GetAuditDaysGridAsync(string startDate, string endDate, List<int> companies, List<string> services, List<int> sites)
        {
            var row = await ExecuteStoredProcedureRowAsync(
                "Sp_GetAuditDaysGrid",
                new Dictionary<string, object?>
                {
                    ["startDate"] = startDate,
                    ["endDate"] = endDate,
                    ["companies"] = JsonSerializer.Serialize(companies),
                    ["services"] = JsonSerializer.Serialize(services),
                    ["sites"] = JsonSerializer.Serialize(sites)
                });

            return RepositoryResponseParser.ParseJsonResponse<AuditDaysGridResponse>(row, "Audit days grid not available");
        }

        public async Task<ApiResponse<AuditDaysByServiceResponse>> GetAuditDaysByServiceAsync(AuditDaysFilter filters)
        {
            var row = await ExecuteStoredProcedureRowAsync(
                "Sp_GetAuditDaysByService",
                new Dictionary<string, object?>
                {
                    ["startDate"] = filters.StartDate,
                    ["endDate"] = filters.EndDate,
                    ["companies"] = JsonSerializer.Serialize(filters.Companies),
                    ["services"] = JsonSerializer.Serialize(filters.Services),
                    ["sites"] = JsonSerializer.Serialize(filters.Sites)
                });

            return RepositoryResponseParser.ParseJsonResponse<AuditDaysByServiceResponse>(row, "Audit days by service not available");
        }

        public async Task<ApiResponse<AuditDaysByMonthAndServiceResponse>> GetAuditDaysByMonthAndServiceAsync(AuditDaysByMonthFilter filters)
        {
            var row = await ExecuteStoredProcedureRowAsync(
                "Sp_GetAuditDaysByMonthAndService",
                new Dictionary<string, object?>
                {
                    ["startDate"] = filters.StartDate,
                    ["endDate"] = filters.EndDate,
                    ["companyFilter"] = JsonSerializer.Serialize(filters.CompanyFilter),
                    ["serviceFilter"] = JsonSerializer.Serialize(filters.ServiceFilter),
                    ["siteFilter"] = JsonSerializer.Serialize(filters.SiteFilter)
                });

            return RepositoryResponseParser.ParseJsonResponse<AuditDaysByMonthAndServiceResponse>(row, "Audit days by month and service not available");
        }

        public async Task<AuditFindingCreatedResponse> CreateAuditFindingAsync(CreateAuditFindingRequest request)
        {
            var findingNumber = $"F-{request.AuditId}-{DateTime.UtcNow:yyyyMMddHHmmss}";
            var findingType = MapFindingType(request.Category);
            var severity = MapSeverity(request.Priority);
            var dueDate = ParseDate(request.DueDate);
            var evidenceJson = request.Evidence.Count == 0 ? null : JsonSerializer.Serialize(request.Evidence, JsonOptions());

            var categoryId = await _dbContext.FindingCategories.AsNoTracking()
                .Where(item => item.CategoryName == request.Category)
                .Select(item => (int?)item.FindingCategoryId)
                .FirstOrDefaultAsync();

            var statusId = await _dbContext.FindingStatuses.AsNoTracking()
                .Where(item => item.StatusName == "Open")
                .Select(item => (int?)item.FindingStatusId)
                .FirstOrDefaultAsync() ?? 1;

            var entity = new FindingEntity
            {
                FindingNumber = findingNumber,
                AuditId = request.AuditId,
                SiteId = request.SiteId,
                Title = request.Title,
                Description = request.Description,
                FindingType = findingType,
                Severity = severity,
                FindingStatusId = statusId,
                FindingCategoryId = categoryId,
                IdentifiedDate = DateTime.UtcNow,
                DueDate = dueDate,
                CreatedBy = request.CreatedBy,
                AssignedTo = request.AssignedToUserId,
                Evidence = evidenceJson,
                RootCause = request.RootCause,
                CorrectiveAction = request.RecommendedAction
            };

            _dbContext.Findings.Add(entity);
            await _dbContext.SaveChangesAsync();

            return new AuditFindingCreatedResponse
            {
                FindingId = entity.FindingId,
                FindingNumber = findingNumber
            };
        }

        private async Task<IDictionary<string, object>?> ExecuteStoredProcedureRowAsync(string procedureName, IDictionary<string, object?> parameters)
        {
            await using var connection = _dbContext.Database.GetDbConnection();
            await EnsureConnectionOpenAsync(connection);

            await using var command = connection.CreateCommand();
            command.CommandText = procedureName;
            command.CommandType = CommandType.StoredProcedure;
            AddParameters(command, parameters);

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return ReadRow(reader);
        }

        private static void AddParameters(DbCommand command, IDictionary<string, object?> parameters)
        {
            foreach (var pair in parameters)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = pair.Key;
                parameter.Value = pair.Value ?? DBNull.Value;
                command.Parameters.Add(parameter);
            }
        }

        private static IDictionary<string, object> ReadRow(DbDataReader reader)
        {
            var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            for (var index = 0; index < reader.FieldCount; index++)
            {
                var value = reader.IsDBNull(index) ? null : reader.GetValue(index);
                result[reader.GetName(index)] = value ?? string.Empty;
            }

            return result;
        }

        private static async Task EnsureConnectionOpenAsync(DbConnection connection)
        {
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }
        }

        private async Task<string?> GetCityNameAsync(int? cityId)
        {
            if (!cityId.HasValue)
            {
                return null;
            }

            return await _dbContext.Cities.AsNoTracking()
                .Where(item => item.CityId == cityId.Value)
                .Select(item => item.CityName)
                .FirstOrDefaultAsync();
        }

        private async Task<string?> GetCountryNameAsync(int? countryId)
        {
            if (!countryId.HasValue)
            {
                return null;
            }

            return await _dbContext.Countries.AsNoTracking()
                .Where(item => item.CountryId == countryId.Value)
                .Select(item => item.CountryName)
                .FirstOrDefaultAsync();
        }

        private static string? BuildSiteAddress(string? address, string? city, string? postCode, string? country)
        {
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(address))
            {
                parts.Add(address.Trim());
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                parts.Add(city.Trim());
            }

            if (!string.IsNullOrWhiteSpace(postCode))
            {
                parts.Add(postCode.Trim());
            }

            if (!string.IsNullOrWhiteSpace(country))
            {
                parts.Add(country.Trim());
            }

            return parts.Count == 0 ? null : string.Join(", ", parts);
        }

        private static string MapFindingType(string? category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return "NC";
            }

            if (category.Contains("Observation", StringComparison.OrdinalIgnoreCase))
            {
                return "Observation";
            }

            if (category.Contains("Opportunity", StringComparison.OrdinalIgnoreCase))
            {
                return "OFI";
            }

            return "NC";
        }

        private static string? MapSeverity(string? priority)
        {
            if (string.IsNullOrWhiteSpace(priority))
            {
                return null;
            }

            if (priority.Equals("Critical", StringComparison.OrdinalIgnoreCase))
            {
                return "Critical";
            }

            if (priority.Equals("Major", StringComparison.OrdinalIgnoreCase))
            {
                return "Major";
            }

            if (priority.Equals("Minor", StringComparison.OrdinalIgnoreCase))
            {
                return "Minor";
            }

            return priority;
        }

        private static DateTime? ParseDate(string? date)
        {
            return DateTime.TryParse(date, out var parsed) ? parsed : null;
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
