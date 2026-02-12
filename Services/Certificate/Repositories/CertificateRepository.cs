using CertificateService.Data;
using CertificateService.Models;
using CertificateService.Models.Rest;
using GraphCertificateListResponse = CertificateService.Models.CertificateListResponse;
using Dapper;
using System.Data;
using System.Text.Json;

namespace CertificateService.Repositories
{
    public class CertificateRepository : ICertificateRepository
    {
        private readonly DapperContext _context;

        public CertificateRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<GraphCertificateListResponse>> GetCertificateListAsync()
        {
            using var connection = _context.CreateConnection();
            var rows = (await connection.QueryAsync<CertificateListRow>(
                "Sp_GetCertificateList",
                commandType: CommandType.StoredProcedure)).ToList();

            var serviceRows = await connection.QueryAsync<CertificateServiceRow>(
                "SELECT CertificateId, ServiceId FROM CertificateServices");
            var siteRows = await connection.QueryAsync<CertificateSiteRow>(
                "SELECT CertificateId, SiteId FROM CertificateSites");

            var serviceMap = serviceRows
                .GroupBy(row => row.CertificateId)
                .ToDictionary(g => g.Key, g => g.Select(r => r.ServiceId).Distinct().ToList());

            var siteMap = siteRows
                .GroupBy(row => row.CertificateId)
                .ToDictionary(g => g.Key, g => g.Select(r => r.SiteId).Distinct().ToList());

            return rows.Select(row => new GraphCertificateListResponse
            {
                CertificateId = row.CertificateId,
                CertificateNumber = row.CertificateNumber,
                CompanyId = row.CompanyId,
                Status = row.Status,
                IssuedDate = row.IssuedDate,
                ValidUntil = row.ValidUntil,
                RevisionNumber = row.RevisionNumber,
                ServiceIds = serviceMap.TryGetValue(row.CertificateId, out var services) ? services : new List<int>(),
                SiteIds = siteMap.TryGetValue(row.CertificateId, out var sites) ? sites : new List<int>()
            }).ToList();
        }

        public async Task<CertificateDetailResponse?> GetCertificateDetailsAsync(int certificateId)
        {
            using var connection = _context.CreateConnection();
            var row = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "Sp_GetCertificateDetails",
                new { certificateId },
                commandType: CommandType.StoredProcedure);

            var parsed = TryParseJsonResponse<CertificateDetailResponse>(row);
            if (parsed?.Data != null)
            {
                return parsed.Data;
            }

            var detail = await connection.QueryFirstOrDefaultAsync<CertificateDetailRow>(
                "SELECT CertificateId, CertificateNumber, CreatedDate, IssueDate, ExpiryDate, RevisionNumber, Status, PreviousCertificateId, Scope, CompanyId, SiteId FROM Certificates WHERE CertificateId = @certificateId",
                new { certificateId });

            if (detail == null)
            {
                return null;
            }

            var serviceNames = await connection.QueryAsync<string>(
                "SELECT s.ServiceName FROM CertificateServices cs JOIN Services s ON cs.ServiceId = s.ServiceId WHERE cs.CertificateId = @certificateId",
                new { certificateId });

            var additionalScopes = await connection.QueryAsync<string>(
                "SELECT ScopeDescription FROM CertificateAdditionalScopes WHERE CertificateId = @certificateId AND IsActive = 1",
                new { certificateId });

            SiteRow? site = null;
            if (detail.SiteId.HasValue)
            {
                site = await connection.QueryFirstOrDefaultAsync<SiteRow>(
                    "SELECT SiteName, Address FROM Sites WHERE SiteId = @siteId",
                    new { siteId = detail.SiteId.Value });
            }

            return new CertificateDetailResponse
            {
                CertificateId = detail.CertificateId,
                CertificateNumber = detail.CertificateNumber,
                CreationDate = detail.CreatedDate,
                IssuedDate = detail.IssueDate,
                NewCertificateId = detail.PreviousCertificateId,
                RevisionNumber = detail.RevisionNumber?.ToString(),
                ScopeInPrimaryLanguage = detail.Scope,
                ScopeInSecondaryLanguage = detail.Scope,
                Services = serviceNames.ToList(),
                ScopeInAdditionalLanguages = additionalScopes
                    .Select(scope => new AdditionalScopeData { Scope = scope })
                    .ToList(),
                SiteNameInPrimaryLanguage = site?.SiteName,
                SiteAddressInPrimaryLanguage = site?.Address,
                Status = detail.Status,
                ValidUntilDate = detail.ExpiryDate
            };
        }

        public async Task<IReadOnlyList<CertificateSiteResponse>> GetCertificateSitesAsync(int certificateId)
        {
            using var connection = _context.CreateConnection();
            var primarySiteId = await connection.QueryFirstOrDefaultAsync<int?>(
                "SELECT SiteId FROM Certificates WHERE CertificateId = @certificateId",
                new { certificateId });

            var rows = await connection.QueryAsync<CertificateSiteDetailRow>(
                "SELECT cs.SiteId, s.SiteName, s.Address, cs.Scope FROM CertificateSites cs JOIN Sites s ON cs.SiteId = s.SiteId WHERE cs.CertificateId = @certificateId",
                new { certificateId });

            return rows.Select(row => new CertificateSiteResponse
            {
                SiteNameInPrimaryLanguage = row.SiteName,
                SiteNameInSecondaryLanguage = row.SiteName,
                SiteAddressInPrimaryLanguage = row.Address,
                SiteAddressInSecondaryLanguage = row.Address,
                SiteScopeInPrimaryLanguage = row.Scope,
                SiteScopeInSecondaryLanguage = row.Scope,
                IsPrimarySite = primarySiteId.HasValue && primarySiteId.Value == row.SiteId
            }).ToList();
        }

        public async Task<PreferenceResponse?> GetPreferencesAsync(string objectType, string objectName, string pageName)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<PreferenceResponse>(
                "Sp_GetPreferences",
                new { objectType, objectName, pageName },
                commandType: CommandType.StoredProcedure);
        }

        public Task<CertificateListPageData> GetCertificateListPageAsync(CertificateListRequest request)
        {
            return Task.FromResult(new CertificateListPageData());
        }

        public Task<CertificateListPageData> SearchCertificatesAsync(CertificateSearchRequest request)
        {
            return Task.FromResult(new CertificateListPageData());
        }

        public Task<CertificateDetailFullResponse?> GetCertificateDetailsFullAsync(int certificateId)
        {
            return Task.FromResult<CertificateDetailFullResponse?>(null);
        }

        public Task<CertificateStatusUpdateResponse?> UpdateCertificateStatusAsync(int certificateId, UpdateCertificateStatusRequest request)
        {
            return Task.FromResult<CertificateStatusUpdateResponse?>(null);
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

        private sealed class CertificateListRow
        {
            public int CertificateId { get; set; }
            public string? CertificateNumber { get; set; }
            public int CompanyId { get; set; }
            public string? Status { get; set; }
            public DateTime? IssuedDate { get; set; }
            public DateTime? ValidUntil { get; set; }
            public string? RevisionNumber { get; set; }
        }

        private sealed class CertificateServiceRow
        {
            public int CertificateId { get; set; }
            public int ServiceId { get; set; }
        }

        private sealed class CertificateSiteRow
        {
            public int CertificateId { get; set; }
            public int SiteId { get; set; }
        }

        private sealed class CertificateDetailRow
        {
            public int CertificateId { get; set; }
            public string? CertificateNumber { get; set; }
            public DateTime? CreatedDate { get; set; }
            public DateTime? IssueDate { get; set; }
            public DateTime? ExpiryDate { get; set; }
            public int? RevisionNumber { get; set; }
            public string? Status { get; set; }
            public int? PreviousCertificateId { get; set; }
            public string? Scope { get; set; }
            public int? CompanyId { get; set; }
            public int? SiteId { get; set; }
        }

        private sealed class CertificateSiteDetailRow
        {
            public int SiteId { get; set; }
            public string? SiteName { get; set; }
            public string? Address { get; set; }
            public string? Scope { get; set; }
        }

        private sealed class SiteRow
        {
            public string? SiteName { get; set; }
            public string? Address { get; set; }
        }
    }
}
