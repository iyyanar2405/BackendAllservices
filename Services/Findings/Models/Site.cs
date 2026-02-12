// Models/Site.cs
namespace FindingsAPI.Gateway
{
    public class Site
    {
        public int SiteId { get; set; }
        public string SiteName { get; set; }
        public int CompanyId { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
    }
}