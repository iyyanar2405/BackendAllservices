namespace AuditService.Models
{
    public class AuditListResponse
    {
        public int AuditId { get; set; }
        public List<int> Sites { get; set; } = new();
        public List<int> Services { get; set; } = new();
        public int CompanyId { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? LeadAuditor { get; set; }
        public string? Type { get; set; }
    }
}
