namespace AuditService.Models
{
    public class AuditDaysByMonthFilter
    {
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public List<int> CompanyFilter { get; set; } = new();
        public List<int> ServiceFilter { get; set; } = new();
        public List<int> SiteFilter { get; set; } = new();
    }
}
