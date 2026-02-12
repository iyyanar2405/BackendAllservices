namespace AuditService.Models
{
    public class AuditDetailResponse
    {
        public int AuditId { get; set; }
        public List<string> AuditorTeam { get; set; } = new();
        public string? EndDate { get; set; }
        public string? LeadAuditor { get; set; }
        public List<string> Services { get; set; } = new();
        public string? SiteAddress { get; set; }
        public string? SiteName { get; set; }
        public string? StartDate { get; set; }
        public string? Status { get; set; }
    }
}
