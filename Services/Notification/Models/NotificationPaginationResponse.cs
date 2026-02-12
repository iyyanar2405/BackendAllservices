namespace NotificationService.Models
{
    public class NotificationPaginationResponse
    {
        public int CurrentPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public List<NotificationItem> Items { get; set; } = new();
    }
}
