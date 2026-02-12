namespace NotificationService.Models
{
    public class NotificationItem
    {
        public DateTime? CreatedTime { get; set; }
        public int? InfoId { get; set; }
        public string? Message { get; set; }
        public string? Language { get; set; }
        public string? NotificationCategory { get; set; }
        public bool? ReadStatus { get; set; }
        public string? Subject { get; set; }
        public string? EntityType { get; set; }
        public string? EntityId { get; set; }
        public string? SnowLink { get; set; }
    }
}
