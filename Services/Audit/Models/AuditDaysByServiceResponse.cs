namespace AuditService.Models
{
    public class AuditDaysByServiceResponse
    {
        public List<AuditDaysByServiceItem> PieChartData { get; set; } = new();
        public decimal TotalServiceAuditsDayCount { get; set; }
    }

    public class AuditDaysByServiceItem
    {
        public decimal AuditDays { get; set; }
        public int AuditPercentage { get; set; }
        public string? ServiceName { get; set; }
    }
}
