using AuditService.Models;

namespace AuditService.Services
{
    public interface IAuditService
    {
        Task<ApiResponse<List<AuditListResponse>>> GetAuditListAsync();
        Task<ApiResponse<AuditDetailResponse>> GetAuditDetailsAsync(int auditId);
        Task<ApiResponse<List<AuditFindingListResponse>>> GetAuditFindingsAsync(int auditId);
        Task<ApiResponse<List<AuditSiteResponse>>> GetAuditSitesAsync(int auditId);
        Task<ApiResponse<List<SubAuditResponse>>> GetSubAuditsAsync(int auditId);
        Task<ApiResponse<AuditDaysGridResponse>> GetAuditDaysGridAsync(string startDate, string endDate, List<int> companies, List<string> services, List<int> sites);
        Task<ApiResponse<AuditDaysByServiceResponse>> GetAuditDaysByServiceAsync(AuditDaysFilter filters);
        Task<ApiResponse<AuditDaysByMonthAndServiceResponse>> GetAuditDaysByMonthAndServiceAsync(AuditDaysByMonthFilter filters);
    }
}
