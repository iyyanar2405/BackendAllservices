namespace AuditService.Models
{
    public class AuditFindingListResponse
    {
        public string? AcceptedDate { get; set; }
        public int AuditId { get; set; }
        public string? Category { get; set; }
        public int CompanyId { get; set; }
        public string? ClosedDate { get; set; }
        public string? DueDate { get; set; }
        public string? FindingNumber { get; set; }
        public int FindingsId { get; set; }
        public string? OpenDate { get; set; }
        public List<string> Services { get; set; } = new();
        public int SiteId { get; set; }
        public string? Status { get; set; }
        public string? Title { get; set; }
    }
}
