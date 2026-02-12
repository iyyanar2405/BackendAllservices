namespace ActionService.Models
{
    public class CreateActionRequest
    {
        public int Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public bool HighPriority { get; set; }
        public string? Message { get; set; }
        public string? Language { get; set; }
        public string? Service { get; set; }
        public string? Site { get; set; }
        public string? EntityType { get; set; }
        public int? EntityId { get; set; }
        public string? Subject { get; set; }
        public string? SnowLink { get; set; }
    }
}
