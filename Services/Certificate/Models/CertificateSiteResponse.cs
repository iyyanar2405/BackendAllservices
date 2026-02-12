namespace CertificateService.Models
{
    public class CertificateSiteResponse
    {
        public string? SiteNameInPrimaryLanguage { get; set; }
        public string? SiteNameInSecondaryLanguage { get; set; }
        public string? SiteAddressInPrimaryLanguage { get; set; }
        public string? SiteAddressInSecondaryLanguage { get; set; }
        public string? SiteScopeInPrimaryLanguage { get; set; }
        public string? SiteScopeInSecondaryLanguage { get; set; }
        public bool IsPrimarySite { get; set; }
    }
}
