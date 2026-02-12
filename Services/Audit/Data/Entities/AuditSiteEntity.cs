namespace AuditService.Data.Entities
{
    public class AuditSiteEntity
    {
        public int AuditSiteId { get; set; }
        public int AuditId { get; set; }
        public int SiteId { get; set; }
        public string? Status { get; set; }
    }
}
