namespace AuditService.Models
{
    public class AuditDaysByMonthAndServiceResponse
    {
        public List<AuditDaysMonthData> ChartData { get; set; } = new();
    }

    public class AuditDaysMonthData
    {
        public string? Month { get; set; }
        public decimal MonthCount { get; set; }
        public List<AuditDaysServiceData> ServiceData { get; set; } = new();
    }

    public class AuditDaysServiceData
    {
        public decimal AuditDays { get; set; }
        public string? ServiceName { get; set; }
    }
}
