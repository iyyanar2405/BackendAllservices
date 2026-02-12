namespace AuditService.Data.Entities
{
    public class SiteEntity
    {
        public int SiteId { get; set; }
        public string SiteName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public int? CityId { get; set; }
        public int? CountryId { get; set; }
        public string? PostalCode { get; set; }
    }
}
