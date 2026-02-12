namespace ScheduleService.Models
{
    public class CalendarScheduleFilterInput
    {
        public List<int> CompanyIds { get; set; } = new();
        public List<int> ServiceIds { get; set; } = new();
        public List<int> SiteIds { get; set; } = new();
        public List<string> Statuses { get; set; } = new();
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
    }
}
