namespace NotificationService.Data
{
    public class NotificationRow
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
        public int CurrentPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }

    public class NotificationSiteRow
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; } = string.Empty;
        public int CityId { get; set; }
        public string CityName { get; set; } = string.Empty;
        public int SiteId { get; set; }
        public string SiteName { get; set; } = string.Empty;
    }
}
