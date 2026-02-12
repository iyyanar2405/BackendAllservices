namespace CertificateService.Models
{
    public class CertificateDetailResponse
    {
        public int CertificateId { get; set; }
        public string? CertificateNumber { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? IssuedDate { get; set; }
        public int? NewCertificateId { get; set; }
        public string? PrimaryLanguage { get; set; }
        public string? RevisionNumber { get; set; }
        public List<AdditionalScopeData> ScopeInAdditionalLanguages { get; set; } = new();
        public string? ScopeInPrimaryLanguage { get; set; }
        public string? ScopeInSecondaryLanguage { get; set; }
        public string? SecondaryLanguage { get; set; }
        public List<string> Services { get; set; } = new();
        public string? SiteAddressInPrimaryLanguage { get; set; }
        public string? SiteNameInPrimaryLanguage { get; set; }
        public string? Status { get; set; }
        public DateTime? SuspendedDate { get; set; }
        public DateTime? ValidUntilDate { get; set; }
        public DateTime? WithdrawnDate { get; set; }
        public string? QRCodeLink { get; set; }
        public string? ProjectNumber { get; set; }
        public string? ReportingCountry { get; set; }
        public string? AccountDNVId { get; set; }
    }

    public class AdditionalScopeData
    {
        public string? Language { get; set; }
        public string? Scope { get; set; }
    }
}
