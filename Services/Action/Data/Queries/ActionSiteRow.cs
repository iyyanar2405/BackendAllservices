namespace ActionService.Data.Queries
{
    public class ActionSiteRow
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; } = string.Empty;
        public int CityId { get; set; }
        public string CityName { get; set; } = string.Empty;
        public int SiteId { get; set; }
        public string SiteName { get; set; } = string.Empty;
    }
}
