namespace AuditService.Models
{
    public class SubAuditResponse
    {
        public int AuditId { get; set; }
        public List<int> Sites { get; set; } = new();
        public List<int> Services { get; set; } = new();
        public string? Status { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public List<string> AuditorTeam { get; set; } = new();
    }
}
