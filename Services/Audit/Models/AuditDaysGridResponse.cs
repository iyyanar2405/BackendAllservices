namespace AuditService.Models
{
    public class AuditDaysGridResponse
    {
        public List<AuditDaysGridNode> Data { get; set; } = new();
    }

    public class AuditDaysGridNode
    {
        public AuditDaysGridNodeData Data { get; set; } = new();
        public List<AuditDaysGridNode> Children { get; set; } = new();
    }

    public class AuditDaysGridNodeData
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public decimal AuditDays { get; set; }
        public string? DataType { get; set; }
    }
}
