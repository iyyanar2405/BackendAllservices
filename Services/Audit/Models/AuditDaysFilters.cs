namespace AuditService.Models
{
    public class AuditDaysFilter
    {
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public List<int> Companies { get; set; } = new();
        public List<string> Services { get; set; } = new();
        public List<int> Sites { get; set; } = new();
    }
}
