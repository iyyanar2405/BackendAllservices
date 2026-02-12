namespace ContractService.Models
{
    public class SiteDetailsResponse
    {
        public int Id { get; set; }
        public string? SiteName { get; set; }
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? City { get; set; }
        public int CountryId { get; set; }
        public string? CountryName { get; set; }
        public string? FormattedAddress { get; set; }
        public string? SiteState { get; set; }
        public string? SiteZip { get; set; }
    }
}
