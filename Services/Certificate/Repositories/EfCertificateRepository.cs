using CertificateService.Data;
using CertificateService.Data.Entities;
using CertificateService.Models;
using CertificateService.Models.Rest;
using GraphCertificateListResponse = CertificateService.Models.CertificateListResponse;
using RestAdditionalScope = CertificateService.Models.Rest.CertificateAdditionalScope;
using Microsoft.EntityFrameworkCore;
using ServiceEntity = CertificateService.Data.Entities.Service;

namespace CertificateService.Repositories
{
    public class EfCertificateRepository : ICertificateRepository
    {
        private const string DefaultCertificatePreference = "{\"filters\":{\"certificateNumber\":[{\"matchMode\":\"in\",\"operator\":\"and\",\"value\":[]}],\"companyName\":[{\"matchMode\":\"in\",\"operator\":\"and\",\"value\":[]}],\"service\":[{\"matchMode\":\"in\",\"operator\":\"and\",\"value\":[]}],\"status\":[{\"matchMode\":\"in\",\"operator\":\"and\",\"value\":[]}],\"validUntil\":[{\"matchMode\":\"dateBefore\",\"operator\":\"and\",\"value\":[]}],\"issuedDate\":[{\"matchMode\":\"dateBefore\",\"operator\":\"and\",\"value\":[]}],\"site\":[{\"matchMode\":\"in\",\"operator\":\"and\",\"value\":[]}],\"city\":[{\"matchMode\":\"in\",\"operator\":\"and\",\"value\":[]}],\"certificateId\":[{\"matchMode\":\"in\",\"operator\":\"and\",\"value\":[]}]},\"rowsPerPage\":10,\"columns\":[{\"field\":\"certificateNumber\",\"displayName\":\"certificate.certificateList.certificateNumber\",\"type\":\"searchCheckboxFilter\",\"cellType\":\"link\",\"hidden\":false,\"fixed\":true,\"sticky\":false,\"routeIdField\":\"certificateId\"},{\"field\":\"certificateId\",\"displayName\":\"certificate.certificateList.certificateId\",\"type\":\"searchCheckboxFilter\",\"cellType\":\"text\",\"hidden\":false,\"fixed\":false,\"sticky\":false},{\"field\":\"companyName\",\"displayName\":\"certificate.certificateList.company\",\"type\":\"searchCheckboxFilter\",\"cellType\":\"text\",\"hidden\":false,\"fixed\":false,\"sticky\":false},{\"field\":\"service\",\"displayName\":\"certificate.certificateList.service\",\"type\":\"searchCheckboxFilter\",\"cellType\":\"text\",\"hidden\":false,\"fixed\":false,\"sticky\":false},{\"field\":\"status\",\"displayName\":\"certificate.certificateList.status\",\"type\":\"checkboxFilter\",\"cellType\":\"status\",\"hidden\":false,\"fixed\":false,\"sticky\":false},{\"field\":\"validUntil\",\"displayName\":\"certificate.certificateList.validUntil\",\"type\":\"dateFilter\",\"cellType\":\"date\",\"hidden\":false,\"fixed\":false,\"sticky\":false},{\"field\":\"issuedDate\",\"displayName\":\"certificate.certificateList.issuedDate\",\"type\":\"dateFilter\",\"cellType\":\"date\",\"hidden\":false,\"fixed\":false,\"sticky\":false},{\"field\":\"site\",\"displayName\":\"certificate.certificateList.site\",\"type\":\"searchCheckboxFilter\",\"cellType\":\"text\",\"hidden\":false,\"fixed\":false,\"sticky\":false},{\"field\":\"city\",\"displayName\":\"certificate.certificateList.city\",\"type\":\"searchCheckboxFilter\",\"cellType\":\"text\",\"hidden\":false,\"fixed\":false,\"sticky\":false}],\"showDefaultColumnsButton\":true}";
        private readonly ApplicationDbContext _context;

        public EfCertificateRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<GraphCertificateListResponse>> GetCertificateListAsync()
        {
            var certificates = await _context.Certificates.AsNoTracking().ToListAsync();
            var certificateIds = certificates.Select(c => c.CertificateId).ToList();

            var serviceMap = await _context.CertificateServices.AsNoTracking()
                .Where(cs => certificateIds.Contains(cs.CertificateId))
                .GroupBy(cs => cs.CertificateId)
                .ToDictionaryAsync(
                    group => group.Key,
                    group => group.Select(cs => cs.ServiceId).Distinct().ToList());

            var siteMap = await _context.CertificateSites.AsNoTracking()
                .Where(cs => certificateIds.Contains(cs.CertificateId))
                .GroupBy(cs => cs.CertificateId)
                .ToDictionaryAsync(
                    group => group.Key,
                    group => group.Select(cs => cs.SiteId).Distinct().ToList());

            return certificates.Select(certificate => new GraphCertificateListResponse
            {
                CertificateId = certificate.CertificateId,
                CertificateNumber = certificate.CertificateNumber,
                CompanyId = certificate.CompanyId,
                Status = certificate.Status,
                IssuedDate = certificate.IssueDate,
                ValidUntil = certificate.ExpiryDate,
                RevisionNumber = certificate.RevisionNumber.ToString(),
                ServiceIds = MergeIds(certificate.ServiceId, serviceMap.TryGetValue(certificate.CertificateId, out var services) ? services : new List<int>()),
                SiteIds = MergeIds(certificate.SiteId, siteMap.TryGetValue(certificate.CertificateId, out var sites) ? sites : new List<int>())
            }).ToList();
        }

        public async Task<CertificateDetailResponse?> GetCertificateDetailsAsync(int certificateId)
        {
            var certificate = await _context.Certificates.AsNoTracking()
                .FirstOrDefaultAsync(c => c.CertificateId == certificateId);

            if (certificate == null)
            {
                return null;
            }

            var serviceIds = await _context.CertificateServices.AsNoTracking()
                .Where(cs => cs.CertificateId == certificateId)
                .Select(cs => cs.ServiceId)
                .ToListAsync();

            var serviceNames = await _context.Set<ServiceEntity>().AsNoTracking()
                .Where(s => serviceIds.Contains(s.ServiceId))
                .Select(s => s.ServiceName)
                .ToListAsync();

            var additionalScopes = await _context.CertificateAdditionalScopes.AsNoTracking()
                .Where(scope => scope.CertificateId == certificateId && scope.IsActive)
                .Select(scope => scope.ScopeDescription)
                .ToListAsync();

            Site? site = null;
            if (certificate.SiteId.HasValue)
            {
                site = await _context.Sites.AsNoTracking()
                    .FirstOrDefaultAsync(s => s.SiteId == certificate.SiteId.Value);
            }

            return new CertificateDetailResponse
            {
                CertificateId = certificate.CertificateId,
                CertificateNumber = certificate.CertificateNumber,
                CreationDate = certificate.CreatedDate,
                IssuedDate = certificate.IssueDate,
                NewCertificateId = certificate.PreviousCertificateId,
                RevisionNumber = certificate.RevisionNumber.ToString(),
                ScopeInPrimaryLanguage = certificate.Scope,
                ScopeInSecondaryLanguage = certificate.Scope,
                Services = serviceNames,
                ScopeInAdditionalLanguages = additionalScopes
                    .Select(scope => new AdditionalScopeData { Scope = scope })
                    .ToList(),
                SiteNameInPrimaryLanguage = site?.SiteName,
                SiteAddressInPrimaryLanguage = site?.Address,
                Status = certificate.Status,
                ValidUntilDate = certificate.ExpiryDate
            };
        }

        public async Task<IReadOnlyList<CertificateSiteResponse>> GetCertificateSitesAsync(int certificateId)
        {
            var primarySiteId = await _context.Certificates.AsNoTracking()
                .Where(c => c.CertificateId == certificateId)
                .Select(c => c.SiteId)
                .FirstOrDefaultAsync();

            var sites = await (from cs in _context.CertificateSites.AsNoTracking()
                               join s in _context.Sites.AsNoTracking() on cs.SiteId equals s.SiteId
                               where cs.CertificateId == certificateId
                               select new CertificateSiteResponse
                               {
                                   SiteNameInPrimaryLanguage = s.SiteName,
                                   SiteNameInSecondaryLanguage = s.SiteName,
                                   SiteAddressInPrimaryLanguage = s.Address,
                                   SiteAddressInSecondaryLanguage = s.Address,
                                   SiteScopeInPrimaryLanguage = cs.Scope,
                                   SiteScopeInSecondaryLanguage = cs.Scope,
                                   IsPrimarySite = primarySiteId.HasValue && primarySiteId.Value == cs.SiteId
                               }).ToListAsync();

            return sites;
        }

        public Task<PreferenceResponse?> GetPreferencesAsync(string objectType, string objectName, string pageName)
        {
            if (string.Equals(objectName, "Certificates", StringComparison.OrdinalIgnoreCase)
                && string.Equals(pageName, "CertificateList", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult<PreferenceResponse?>(new PreferenceResponse
                {
                    PageName = pageName,
                    ObjectType = objectType,
                    ObjectName = objectName,
                    PreferenceDetail = DefaultCertificatePreference
                });
            }

            return Task.FromResult<PreferenceResponse?>(null);
        }

        public async Task<CertificateListPageData> GetCertificateListPageAsync(CertificateListRequest request)
        {
            var baseQuery = BuildBaseListQuery();
            baseQuery = ApplyFilters(baseQuery, request.Filters);

            var totalCount = await baseQuery.CountAsync();
            var statusCounts = await baseQuery
                .GroupBy(row => row.Certificate.Status)
                .Select(group => new { Status = group.Key, Total = group.Count() })
                .ToDictionaryAsync(row => row.Status ?? "Unknown", row => row.Total);

            var today = DateTime.UtcNow.Date;
            var expiring30 = await baseQuery.CountAsync(row => row.Certificate.ExpiryDate >= today && row.Certificate.ExpiryDate <= today.AddDays(30));
            var expiring90 = await baseQuery.CountAsync(row => row.Certificate.ExpiryDate >= today && row.Certificate.ExpiryDate <= today.AddDays(90));

            var ordered = ApplyOrdering(baseQuery, request.SortBy, request.SortDirection);
            var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;
            var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
            var paged = ordered.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            var items = await paged.Select(row => new CertificateListItemResponse
            {
                CertificateId = row.Certificate.CertificateId,
                CertificateNumber = row.Certificate.CertificateNumber,
                CompanyId = row.Certificate.CompanyId,
                CompanyName = row.CompanyName,
                Status = row.Certificate.Status,
                IssuedDate = row.Certificate.IssueDate,
                ValidFrom = row.Certificate.IssueDate,
                ValidUntil = row.Certificate.ExpiryDate,
                RevisionNumber = row.Certificate.RevisionNumber.ToString(),
                CertificateType = row.Certificate.CertificateType,
                Country = row.CountryName,
                CreatedDate = row.Certificate.CreatedDate,
                ModifiedDate = row.Certificate.ModifiedDate,
                ServiceIds = new List<int> { row.Certificate.ServiceId },
                SiteIds = row.Certificate.SiteId.HasValue ? new List<int> { row.Certificate.SiteId.Value } : new List<int>()
            }).ToListAsync();

            await PopulateListChildrenAsync(items);

            return new CertificateListPageData
            {
                Items = items,
                TotalCount = totalCount,
                StatusCounts = statusCounts,
                ExpiringWithin30Days = expiring30,
                ExpiringWithin90Days = expiring90
            };
        }

        public async Task<CertificateListPageData> SearchCertificatesAsync(CertificateSearchRequest request)
        {
            var baseQuery = BuildBaseListQuery();
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                baseQuery = baseQuery.Where(row => row.Certificate.CertificateNumber.Contains(request.SearchTerm)
                    || (row.CompanyName ?? string.Empty).Contains(request.SearchTerm));
            }

            var filters = new CertificateListFilters
            {
                CompanyIds = request.CompanyIds,
                SiteIds = request.SiteIds,
                ServiceIds = request.ServiceIds,
                Statuses = request.Statuses
            };

            baseQuery = ApplyFilters(baseQuery, filters);

            if (request.CertificateTypes.Any())
            {
                baseQuery = baseQuery.Where(row => request.CertificateTypes.Contains(row.Certificate.CertificateType ?? string.Empty));
            }

            if (request.Countries.Any())
            {
                baseQuery = baseQuery.Where(row => request.Countries.Contains(row.CountryName ?? string.Empty)
                    || request.Countries.Contains(row.CountryCode ?? string.Empty)
                    || request.Countries.Contains(row.CountryCodeAlpha2 ?? string.Empty));
            }

            if (request.Standards.Any())
            {
                var standardServiceIds = await _context.Set<ServiceEntity>().AsNoTracking()
                    .Where(s => request.Standards.Contains(s.ServiceCode) || request.Standards.Contains(s.ServiceName))
                    .Select(s => s.ServiceId)
                    .ToListAsync();

                var standardIds = await _context.CertificateServices.AsNoTracking()
                    .Where(cs => standardServiceIds.Contains(cs.ServiceId))
                    .Select(cs => cs.CertificateId)
                    .Distinct()
                    .ToListAsync();

                baseQuery = baseQuery.Where(row => standardIds.Contains(row.Certificate.CertificateId));
            }

            if (request.ExpiryPeriod?.WithinDays != null)
            {
                var limit = DateTime.UtcNow.Date.AddDays(request.ExpiryPeriod.WithinDays.Value);
                baseQuery = baseQuery.Where(row => row.Certificate.ExpiryDate <= limit);
            }

            var totalCount = await baseQuery.CountAsync();
            var statusCounts = await baseQuery
                .GroupBy(row => row.Certificate.Status)
                .Select(group => new { Status = group.Key, Total = group.Count() })
                .ToDictionaryAsync(row => row.Status ?? "Unknown", row => row.Total);

            var today = DateTime.UtcNow.Date;
            var expiring30 = await baseQuery.CountAsync(row => row.Certificate.ExpiryDate >= today && row.Certificate.ExpiryDate <= today.AddDays(30));
            var expiring90 = await baseQuery.CountAsync(row => row.Certificate.ExpiryDate >= today && row.Certificate.ExpiryDate <= today.AddDays(90));

            var ordered = ApplyOrdering(baseQuery, request.SortBy, request.SortDirection);
            var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;
            var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
            var paged = ordered.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            var items = await paged.Select(row => new CertificateListItemResponse
            {
                CertificateId = row.Certificate.CertificateId,
                CertificateNumber = row.Certificate.CertificateNumber,
                CompanyId = row.Certificate.CompanyId,
                CompanyName = row.CompanyName,
                Status = row.Certificate.Status,
                IssuedDate = row.Certificate.IssueDate,
                ValidFrom = row.Certificate.IssueDate,
                ValidUntil = row.Certificate.ExpiryDate,
                RevisionNumber = row.Certificate.RevisionNumber.ToString(),
                CertificateType = row.Certificate.CertificateType,
                Country = row.CountryName,
                CreatedDate = row.Certificate.CreatedDate,
                ModifiedDate = row.Certificate.ModifiedDate,
                ServiceIds = new List<int> { row.Certificate.ServiceId },
                SiteIds = row.Certificate.SiteId.HasValue ? new List<int> { row.Certificate.SiteId.Value } : new List<int>()
            }).ToListAsync();

            await PopulateListChildrenAsync(items);

            return new CertificateListPageData
            {
                Items = items,
                TotalCount = totalCount,
                StatusCounts = statusCounts,
                ExpiringWithin30Days = expiring30,
                ExpiringWithin90Days = expiring90
            };
        }

        public async Task<CertificateDetailFullResponse?> GetCertificateDetailsFullAsync(int certificateId)
        {
            var row = await BuildDetailQuery(certificateId).FirstOrDefaultAsync();
            if (row == null)
            {
                return null;
            }

            var certificateServiceRows = await _context.CertificateServices.AsNoTracking()
                .Where(cs => cs.CertificateId == certificateId)
                .Select(cs => new { cs.ServiceId, cs.Scope })
                .ToListAsync();

            var serviceLookup = await _context.Set<ServiceEntity>().AsNoTracking()
                .Where(s => certificateServiceRows.Select(row => row.ServiceId).Contains(s.ServiceId))
                .ToDictionaryAsync(s => s.ServiceId);

            var services = certificateServiceRows.Select(row => new CertificateServiceDetail
            {
                ServiceId = row.ServiceId,
                ServiceName = serviceLookup.TryGetValue(row.ServiceId, out var service) ? service.ServiceName : null,
                Standard = serviceLookup.TryGetValue(row.ServiceId, out var codeService) ? codeService.ServiceCode : null
            }).ToList();

            var additionalScopes = await _context.CertificateAdditionalScopes.AsNoTracking()
                .Where(scope => scope.CertificateId == certificateId && scope.IsActive)
                .Select(scope => scope.ScopeDescription)
                .ToListAsync();

            var audits = new List<CertificateAuditSummary>();
            if (row.Certificate.AuditId.HasValue)
            {
                var audit = await _context.Audits.AsNoTracking()
                    .FirstOrDefaultAsync(a => a.AuditId == row.Certificate.AuditId.Value);

                if (audit != null)
                {
                    audits.Add(new CertificateAuditSummary
                    {
                        AuditId = audit.AuditId,
                        AuditType = audit.Type,
                        AuditDate = audit.StartDate,
                        LeadAuditor = audit.LeadAuditor,
                        Status = audit.Status,
                        FindingsCount = 0
                    });
                }
            }

            return new CertificateDetailFullResponse
            {
                CertificateId = row.Certificate.CertificateId,
                CertificateNumber = row.Certificate.CertificateNumber,
                Status = row.Certificate.Status,
                CertificateType = row.Certificate.CertificateType,
                CreationDate = row.Certificate.CreatedDate,
                IssuedDate = row.Certificate.IssueDate,
                ValidFromDate = row.Certificate.IssueDate,
                ValidUntilDate = row.Certificate.ExpiryDate,
                RevisionNumber = row.Certificate.RevisionNumber.ToString(),
                NewCertificateId = row.Certificate.PreviousCertificateId,
                QRCodeLink = row.Certificate.CertificatePath,
                Company = new CertificateCompanySummary
                {
                    CompanyId = row.Certificate.CompanyId,
                    CompanyName = row.CompanyName,
                    ReportingCountry = row.CountryCode,
                    ContactPerson = row.ContactPerson,
                    ContactEmail = row.ContactEmail
                },
                Site = row.SiteId.HasValue
                    ? new CertificateSiteDetail
                    {
                        SiteId = row.SiteId.Value,
                        SiteNameInPrimaryLanguage = row.SiteName,
                        SiteAddressInPrimaryLanguage = row.SiteAddress,
                        SiteNameInSecondaryLanguage = row.SiteName,
                        SiteAddressInSecondaryLanguage = row.SiteAddress
                    }
                    : null,
                Services = services,
                Scope = new CertificateScopeDetail
                {
                    ScopeInPrimaryLanguage = row.Certificate.Scope,
                    ScopeInSecondaryLanguage = row.Certificate.Scope,
                    ScopeInAdditionalLanguages = additionalScopes
                        .Select(scope => new RestAdditionalScope { Scope = scope })
                        .ToList()
                },
                Audits = audits,
                Renewal = BuildRenewal(row.Certificate.ExpiryDate),
                Verification = new CertificateVerificationSummary(),
                CreatedDate = row.Certificate.CreatedDate,
                CreatedBy = row.Certificate.CreatedBy?.ToString(),
                ModifiedDate = row.Certificate.ModifiedDate,
                ModifiedBy = row.Certificate.ModifiedBy?.ToString()
            };
        }

        public async Task<CertificateStatusUpdateResponse?> UpdateCertificateStatusAsync(int certificateId, UpdateCertificateStatusRequest request)
        {
            var certificate = await _context.Certificates.FirstOrDefaultAsync(c => c.CertificateId == certificateId);
            if (certificate == null)
            {
                return null;
            }

            certificate.Status = request.NewStatus ?? certificate.Status;
            certificate.ModifiedDate = request.EffectiveDate ?? DateTime.UtcNow;
            certificate.ModifiedBy = request.ModifiedBy;

            await _context.SaveChangesAsync();

            return new CertificateStatusUpdateResponse
            {
                CertificateId = certificate.CertificateId,
                Status = certificate.Status,
                ModifiedDate = certificate.ModifiedDate
            };
        }

        private IQueryable<ListRow> BuildBaseListQuery()
        {
            return from c in _context.Certificates.AsNoTracking()
                   join co in _context.Companies.AsNoTracking() on c.CompanyId equals co.CompanyId into companies
                   from co in companies.DefaultIfEmpty()
                   join cn in _context.Countries.AsNoTracking() on co.CountryId equals cn.CountryId into countries
                   from cn in countries.DefaultIfEmpty()
                   select new ListRow
                   {
                       Certificate = c,
                       CompanyName = co != null ? co.CompanyName : null,
                       CountryName = cn != null ? cn.CountryName : null,
                       CountryCode = cn != null ? cn.CountryCode : null,
                       CountryCodeAlpha2 = cn != null ? cn.CountryCodeAlpha2 : null,
                       ContactPerson = co != null ? co.ContactPerson : null,
                       ContactEmail = co != null ? co.ContactEmail : null,
                       SiteId = c.SiteId
                   };
        }

        private IQueryable<ListRow> ApplyFilters(IQueryable<ListRow> query, CertificateListFilters filters)
        {
            if (filters.CompanyIds.Any())
            {
                query = query.Where(row => filters.CompanyIds.Contains(row.Certificate.CompanyId));
            }

            if (filters.CertificateNumbers.Any())
            {
                query = query.Where(row => filters.CertificateNumbers.Contains(row.Certificate.CertificateNumber));
            }

            if (filters.Statuses.Any())
            {
                var statuses = MapStatuses(filters.Statuses);
                query = query.Where(row => statuses.Contains(row.Certificate.Status));
            }

            if (filters.IncludeSuspended == false)
            {
                query = query.Where(row => row.Certificate.Status != "Suspended");
            }

            if (filters.IncludeExpired == false)
            {
                query = query.Where(row => row.Certificate.Status != "Expired");
            }

            if (filters.IssuedDateRange?.StartDate != null)
            {
                query = query.Where(row => row.Certificate.IssueDate >= filters.IssuedDateRange.StartDate.Value);
            }

            if (filters.IssuedDateRange?.EndDate != null)
            {
                query = query.Where(row => row.Certificate.IssueDate <= filters.IssuedDateRange.EndDate.Value);
            }

            if (filters.ExpiryDateRange?.StartDate != null)
            {
                query = query.Where(row => row.Certificate.ExpiryDate >= filters.ExpiryDateRange.StartDate.Value);
            }

            if (filters.ExpiryDateRange?.EndDate != null)
            {
                query = query.Where(row => row.Certificate.ExpiryDate <= filters.ExpiryDateRange.EndDate.Value);
            }

            if (filters.ServiceIds.Any())
            {
                query = query.Where(row => filters.ServiceIds.Contains(row.Certificate.ServiceId)
                    || _context.CertificateServices.AsNoTracking()
                        .Any(cs => cs.CertificateId == row.Certificate.CertificateId
                            && filters.ServiceIds.Contains(cs.ServiceId)));
            }

            if (filters.SiteIds.Any())
            {
                query = query.Where(row => (row.Certificate.SiteId.HasValue && filters.SiteIds.Contains(row.Certificate.SiteId.Value))
                    || _context.CertificateSites.AsNoTracking()
                        .Any(cs => cs.CertificateId == row.Certificate.CertificateId
                            && filters.SiteIds.Contains(cs.SiteId)));
            }

            return query;
        }

        private static IOrderedQueryable<ListRow> ApplyOrdering(IQueryable<ListRow> query, string? sortBy, string? sortDirection)
        {
            var descending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

            return (sortBy ?? string.Empty).ToLowerInvariant() switch
            {
                "issueddate" => descending
                    ? query.OrderByDescending(row => row.Certificate.IssueDate)
                    : query.OrderBy(row => row.Certificate.IssueDate),
                "validuntil" => descending
                    ? query.OrderByDescending(row => row.Certificate.ExpiryDate)
                    : query.OrderBy(row => row.Certificate.ExpiryDate),
                "certificatenumber" => descending
                    ? query.OrderByDescending(row => row.Certificate.CertificateNumber)
                    : query.OrderBy(row => row.Certificate.CertificateNumber),
                "status" => descending
                    ? query.OrderByDescending(row => row.Certificate.Status)
                    : query.OrderBy(row => row.Certificate.Status),
                "companyname" => descending
                    ? query.OrderByDescending(row => row.CompanyName)
                    : query.OrderBy(row => row.CompanyName),
                _ => descending
                    ? query.OrderByDescending(row => row.Certificate.CertificateId)
                    : query.OrderBy(row => row.Certificate.CertificateId)
            };
        }

        private async Task PopulateListChildrenAsync(List<CertificateListItemResponse> items)
        {
            var certificateIds = items.Select(item => item.CertificateId).ToList();
            var serviceRows = await _context.CertificateServices.AsNoTracking()
                .Where(cs => certificateIds.Contains(cs.CertificateId))
                .Select(cs => new { cs.CertificateId, cs.ServiceId, cs.Scope })
                .ToListAsync();

            var serviceIds = serviceRows.Select(row => row.ServiceId).Distinct().ToList();
            var serviceInfo = await _context.Set<ServiceEntity>().AsNoTracking()
                .Where(s => serviceIds.Contains(s.ServiceId))
                .ToDictionaryAsync(s => s.ServiceId);

            var siteRows = await (from cs in _context.CertificateSites.AsNoTracking()
                                  join s in _context.Sites.AsNoTracking() on cs.SiteId equals s.SiteId
                                  where certificateIds.Contains(cs.CertificateId)
                                  select new
                                  {
                                      cs.CertificateId,
                                      s.SiteId,
                                      s.SiteName,
                                      s.Address
                                  }).ToListAsync();

            var serviceMap = serviceRows.GroupBy(row => row.CertificateId)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(row => new CertificateServiceSummary
                    {
                        ServiceId = row.ServiceId,
                        ServiceName = serviceInfo.TryGetValue(row.ServiceId, out var service) ? service.ServiceName : null,
                        Standard = serviceInfo.TryGetValue(row.ServiceId, out var standard) ? standard.ServiceCode : null,
                        Scope = row.Scope
                    }).ToList());

            var siteMap = siteRows.GroupBy(row => row.CertificateId)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(row => new CertificateSiteSummary
                    {
                        SiteId = row.SiteId,
                        SiteName = row.SiteName,
                        SiteAddress = row.Address
                    }).ToList());

            foreach (var item in items)
            {
                if (serviceMap.TryGetValue(item.CertificateId, out var services))
                {
                    item.Services = services;
                    var ids = services.Select(service => service.ServiceId).ToList();
                    ids.AddRange(item.ServiceIds);
                    item.ServiceIds = ids.Distinct().ToList();
                }

                if (siteMap.TryGetValue(item.CertificateId, out var sites))
                {
                    item.Sites = sites;
                    var ids = sites.Select(site => site.SiteId).ToList();
                    ids.AddRange(item.SiteIds);
                    item.SiteIds = ids.Distinct().ToList();
                }
            }
        }

        private IQueryable<DetailRow> BuildDetailQuery(int certificateId)
        {
            return from c in _context.Certificates.AsNoTracking()
                   join co in _context.Companies.AsNoTracking() on c.CompanyId equals co.CompanyId into companies
                   from co in companies.DefaultIfEmpty()
                   join cn in _context.Countries.AsNoTracking() on co.CountryId equals cn.CountryId into countries
                   from cn in countries.DefaultIfEmpty()
                   join s in _context.Sites.AsNoTracking() on c.SiteId equals s.SiteId into sites
                   from s in sites.DefaultIfEmpty()
                   where c.CertificateId == certificateId
                   select new DetailRow
                   {
                       Certificate = c,
                       CompanyName = co != null ? co.CompanyName : null,
                       CountryCode = cn != null ? cn.CountryCode : null,
                       ContactPerson = co != null ? co.ContactPerson : null,
                       ContactEmail = co != null ? co.ContactEmail : null,
                       SiteId = s != null ? s.SiteId : null,
                       SiteName = s != null ? s.SiteName : null,
                       SiteAddress = s != null ? s.Address : null
                   };
        }

        private static CertificateRenewalSummary BuildRenewal(DateTime? expiryDate)
        {
            if (!expiryDate.HasValue)
            {
                return new CertificateRenewalSummary();
            }

            var today = DateTime.UtcNow.Date;
            var daysUntil = (int)Math.Floor((expiryDate.Value.Date - today).TotalDays);
            return new CertificateRenewalSummary
            {
                RenewalRequired = daysUntil <= 90,
                RenewalDueDate = expiryDate.Value.Date.AddDays(-90),
                RenewalStatus = daysUntil <= 90 ? "Pending" : "Not Required",
                DaysUntilRenewal = daysUntil
            };
        }

        private static List<int> MergeIds(int? primaryId, List<int> ids)
        {
            if (primaryId.HasValue && !ids.Contains(primaryId.Value))
            {
                ids.Add(primaryId.Value);
            }

            return ids;
        }

        private static List<string> MapStatuses(IEnumerable<string> statuses)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Valid"] = "Active",
                ["In Progress"] = "In Progress",
                ["Suspended"] = "Suspended",
                ["Withdrawn"] = "Withdrawn",
                ["Expired"] = "Expired",
                ["Active"] = "Active",
                ["Cancelled"] = "Cancelled"
            };

            return statuses.Select(status => map.TryGetValue(status, out var mapped) ? mapped : status)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private sealed class ListRow
        {
            public Certificate Certificate { get; set; } = new();
            public string? CompanyName { get; set; }
            public string? CountryName { get; set; }
            public string? CountryCode { get; set; }
            public string? CountryCodeAlpha2 { get; set; }
            public string? ContactPerson { get; set; }
            public string? ContactEmail { get; set; }
            public int? SiteId { get; set; }
        }

        private sealed class DetailRow
        {
            public Certificate Certificate { get; set; } = new();
            public string? CompanyName { get; set; }
            public string? CountryCode { get; set; }
            public string? ContactPerson { get; set; }
            public string? ContactEmail { get; set; }
            public int? SiteId { get; set; }
            public string? SiteName { get; set; }
            public string? SiteAddress { get; set; }
        }
    }
}
