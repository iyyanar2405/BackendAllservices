namespace AuditService.Models
{
    public class CreateAuditFindingRequest
    {
        public int AuditId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? Priority { get; set; }
        public string? Clause { get; set; }
        public string? Standard { get; set; }
        public int? SiteId { get; set; }
        public List<int> ServiceIds { get; set; } = new();
        public string? DueDate { get; set; }
        public int? AssignedToUserId { get; set; }
        public string? RootCause { get; set; }
        public string? RecommendedAction { get; set; }
        public List<CreateAuditFindingEvidence> Evidence { get; set; } = new();
        public int? CreatedBy { get; set; }
    }

    public class CreateAuditFindingEvidence
    {
        public string? Description { get; set; }
        public string? FileName { get; set; }
        public string? FileData { get; set; }
    }
}
