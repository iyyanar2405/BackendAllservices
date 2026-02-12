namespace ScheduleService.Models
{
    public class AuditScheduleResponse
    {
        public int SiteAuditId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public List<int> ServiceIds { get; set; } = new();
        public int SiteId { get; set; }
        public string? AuditType { get; set; }
        public string? LeadAuditor { get; set; }
        public List<string> SiteRepresentatives { get; set; } = new();
        public int CompanyId { get; set; }
        public int AuditId { get; set; }
        public string? ReportingCountry { get; set; }
        public string? ProjectNumber { get; set; }
        public string? AccountDNVId { get; set; }
    }
}
