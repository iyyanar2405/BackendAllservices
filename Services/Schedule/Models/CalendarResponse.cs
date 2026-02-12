namespace ScheduleService.Models
{
    public class CalendarResponse
    {
        public List<int> IcsResponse { get; set; } = new();
        public CalendarAttributes? CalendarAttributes { get; set; }
    }

    public class CalendarAttributes
    {
        public string? AuditType { get; set; }
        public DateTime? EndDate { get; set; }
        public string? LeadAuditor { get; set; }
        public string? Service { get; set; }
        public string? Site { get; set; }
        public string? SiteAddress { get; set; }
        public string? SiteRepresentative { get; set; }
        public DateTime? StartDate { get; set; }
    }
}
