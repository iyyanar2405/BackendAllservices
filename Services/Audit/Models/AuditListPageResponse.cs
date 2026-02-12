namespace AuditService.Models
{
    public class AuditListPageResponse
    {
        public List<AuditListResponse> Audits { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
}
