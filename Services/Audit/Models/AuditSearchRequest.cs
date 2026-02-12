namespace AuditService.Models
{
    public class AuditSearchRequest
    {
        public string? SearchTerm { get; set; }
        public List<int> CompanyIds { get; set; } = new();
        public List<int> SiteIds { get; set; } = new();
        public List<int> ServiceIds { get; set; } = new();
        public List<string> AuditStatuses { get; set; } = new();
        public List<string> AuditTypes { get; set; } = new();
        public List<string> LeadAuditors { get; set; } = new();
        public AuditDateRangeFilter? DateRange { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }
    }

    public class AuditDateRangeFilter
    {
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
    }
}
