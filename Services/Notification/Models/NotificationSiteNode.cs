namespace NotificationService.Models
{
    public class NotificationSiteNode
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public List<NotificationSiteNode> Children { get; set; } = new();
    }
}
