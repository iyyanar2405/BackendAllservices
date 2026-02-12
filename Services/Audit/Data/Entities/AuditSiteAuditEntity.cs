namespace AuditService.Data.Entities
{
    public class AuditSiteAuditEntity
    {
        public int AuditSiteAuditId { get; set; }
        public int AuditId { get; set; }
        public int SiteId { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
