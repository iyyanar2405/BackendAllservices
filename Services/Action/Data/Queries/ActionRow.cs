namespace ActionService.Data.Queries
{
    public class ActionRow
    {
        public int Id { get; set; }
        public string? Action { get; set; }
        public DateTime? DueDate { get; set; }
        public bool HighPriority { get; set; }
        public string? Message { get; set; }
        public string? Language { get; set; }
        public string? Service { get; set; }
        public string? Site { get; set; }
        public string? EntityType { get; set; }
        public string? EntityId { get; set; }
        public string? Subject { get; set; }
        public string? SnowLink { get; set; }
        public int CurrentPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}
