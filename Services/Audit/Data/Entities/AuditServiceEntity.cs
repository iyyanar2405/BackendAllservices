namespace AuditService.Data.Entities
{
    public class AuditServiceEntity
    {
        public int AuditServiceId { get; set; }
        public int AuditId { get; set; }
        public int ServiceId { get; set; }
        public string? Status { get; set; }
    }
}
