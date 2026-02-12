using AuditService.Models;
using AuditService.Services;

namespace AuditService.GraphQL.Queries
{
    public class Query
    {
        private readonly IAuditService _service;

        public Query(IAuditService service)
        {
            _service = service;
        }

        [GraphQLName("viewAudits")]
        public Task<ApiResponse<List<AuditListResponse>>> ViewAudits()
        {
            return _service.GetAuditListAsync();
        }

        [GraphQLName("auditDetails")]
        public Task<ApiResponse<AuditDetailResponse>> AuditDetails(int auditId)
        {
            return _service.GetAuditDetailsAsync(auditId);
        }

        [GraphQLName("viewFindings")]
        public Task<ApiResponse<List<AuditFindingListResponse>>> ViewFindings(int auditId)
        {
            return _service.GetAuditFindingsAsync(auditId);
        }

        [GraphQLName("viewSitesForAudit")]
        public Task<ApiResponse<List<AuditSiteResponse>>> ViewSitesForAudit(int auditId)
        {
            return _service.GetAuditSitesAsync(auditId);
        }

        [GraphQLName("viewSubAudits")]
        public Task<ApiResponse<List<SubAuditResponse>>> ViewSubAudits(int auditId)
        {
            return _service.GetSubAuditsAsync(auditId);
        }

        [GraphQLName("getAuditDaysPerSite")]
        public Task<ApiResponse<AuditDaysGridResponse>> GetAuditDaysPerSite(
            string startDate,
            string endDate,
            List<int>? companies,
            List<int>? services,
            List<int>? sites)
        {
            return _service.GetAuditDaysGridAsync(
                startDate,
                endDate,
                companies ?? new List<int>(),
                services?.Select(id => id.ToString()).ToList() ?? new List<string>(),
                sites ?? new List<int>());
        }

        [GraphQLName("auditDaysbyServicePieChart")]
        public Task<ApiResponse<AuditDaysByServiceResponse>> AuditDaysByServicePieChart(AuditDaysFilter filters)
        {
            return _service.GetAuditDaysByServiceAsync(filters);
        }

        [GraphQLName("getAuditDaysByMonthAndService")]
        public Task<ApiResponse<AuditDaysByMonthAndServiceResponse>> GetAuditDaysByMonthAndService(
            string startDate,
            string endDate,
            List<int>? companyFilter,
            List<int>? serviceFilter,
            List<int>? siteFilter)
        {
            return _service.GetAuditDaysByMonthAndServiceAsync(new AuditDaysByMonthFilter
            {
                StartDate = startDate,
                EndDate = endDate,
                CompanyFilter = companyFilter ?? new List<int>(),
                ServiceFilter = serviceFilter ?? new List<int>(),
                SiteFilter = siteFilter ?? new List<int>()
            });
        }
    }
}
