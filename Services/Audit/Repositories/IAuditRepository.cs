using AuditService.Models;

namespace AuditService.Repositories
{
    public interface IAuditRepository
    {
        Task<IReadOnlyList<AuditListResponse>> GetAuditListAsync();
        Task<AuditDetailResponse?> GetAuditDetailsAsync(int auditId);
        Task<IReadOnlyList<AuditFindingListResponse>> GetAuditFindingsAsync(int auditId);
        Task<IReadOnlyList<AuditSiteResponse>> GetAuditSitesAsync(int auditId);
        Task<IReadOnlyList<SubAuditResponse>> GetSubAuditsAsync(int auditId);
        Task<ApiResponse<AuditDaysGridResponse>> GetAuditDaysGridAsync(string startDate, string endDate, List<int> companies, List<string> services, List<int> sites);
        Task<ApiResponse<AuditDaysByServiceResponse>> GetAuditDaysByServiceAsync(AuditDaysFilter filters);
        Task<ApiResponse<AuditDaysByMonthAndServiceResponse>> GetAuditDaysByMonthAndServiceAsync(AuditDaysByMonthFilter filters);
    }
}
