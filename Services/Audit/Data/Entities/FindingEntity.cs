namespace AuditService.Data.Entities
{
    public class FindingEntity
    {
        public int FindingId { get; set; }
        public string FindingNumber { get; set; } = string.Empty;
        public int AuditId { get; set; }
        public int? SiteId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string FindingType { get; set; } = string.Empty;
        public string? Severity { get; set; }
        public int FindingStatusId { get; set; }
        public int? FindingCategoryId { get; set; }
        public DateTime IdentifiedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? AssignedTo { get; set; }
        public string? Evidence { get; set; }
        public string? RootCause { get; set; }
        public string? CorrectiveAction { get; set; }
    }
}
