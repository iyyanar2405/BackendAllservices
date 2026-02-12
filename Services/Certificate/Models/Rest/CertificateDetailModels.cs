using System.Text.Json.Serialization;

namespace CertificateService.Models.Rest
{
    public class CertificateDetailFullResponse
    {
        public int CertificateId { get; set; }
        public string? CertificateNumber { get; set; }
        public string? Status { get; set; }
        public string? CertificateType { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? IssuedDate { get; set; }
        public DateTime? ValidFromDate { get; set; }
        public DateTime? ValidUntilDate { get; set; }
        public DateTime? SuspendedDate { get; set; }
        public DateTime? WithdrawnDate { get; set; }
        public string? RevisionNumber { get; set; }
        public int? NewCertificateId { get; set; }
        public string? ProjectNumber { get; set; }
        [JsonPropertyName("qRCodeLink")]
        public string? QRCodeLink { get; set; }
        public CertificateCompanySummary? Company { get; set; }
        public CertificateSiteDetail? Site { get; set; }
        public List<CertificateServiceDetail> Services { get; set; } = new();
        public CertificateScopeDetail Scope { get; set; } = new();
        public List<CertificateAuditSummary> Audits { get; set; } = new();
        public List<CertificateDocumentSummary> Documents { get; set; } = new();
        public List<CertificateHistorySummary> History { get; set; } = new();
        public CertificateRenewalSummary Renewal { get; set; } = new();
        public CertificateVerificationSummary Verification { get; set; } = new();
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public class CertificateCompanySummary
    {
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? ReportingCountry { get; set; }
        [JsonPropertyName("accountDNVId")]
        public string? AccountDNVId { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactEmail { get; set; }
    }

    public class CertificateSiteDetail
    {
        public int SiteId { get; set; }
        public string? SiteNameInPrimaryLanguage { get; set; }
        public string? SiteAddressInPrimaryLanguage { get; set; }
        public string? SiteNameInSecondaryLanguage { get; set; }
        public string? SiteAddressInSecondaryLanguage { get; set; }
    }

    public class CertificateServiceDetail
    {
        public int ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public string? Standard { get; set; }
        public string? AccreditationBody { get; set; }
    }

    public class CertificateScopeDetail
    {
        public string? PrimaryLanguage { get; set; }
        public string? ScopeInPrimaryLanguage { get; set; }
        public string? SecondaryLanguage { get; set; }
        public string? ScopeInSecondaryLanguage { get; set; }
        public List<CertificateAdditionalScope> ScopeInAdditionalLanguages { get; set; } = new();
    }

    public class CertificateAdditionalScope
    {
        public string? Language { get; set; }
        public string? Scope { get; set; }
    }

    public class CertificateAuditSummary
    {
        public int AuditId { get; set; }
        public string? AuditType { get; set; }
        public DateTime? AuditDate { get; set; }
        public string? LeadAuditor { get; set; }
        public string? Status { get; set; }
        public int? FindingsCount { get; set; }
    }

    public class CertificateDocumentSummary
    {
        public int DocumentId { get; set; }
        public string? DocumentName { get; set; }
        public string? DocumentType { get; set; }
        public string? Language { get; set; }
        public DateTime? UploadDate { get; set; }
        public string? FileSize { get; set; }
        public string? DownloadUrl { get; set; }
    }

    public class CertificateHistorySummary
    {
        public int HistoryId { get; set; }
        public string? Action { get; set; }
        public DateTime? Date { get; set; }
        public string? PerformedBy { get; set; }
        public string? Details { get; set; }
    }

    public class CertificateRenewalSummary
    {
        public bool RenewalRequired { get; set; }
        public DateTime? RenewalDueDate { get; set; }
        public string? RenewalStatus { get; set; }
        public int? DaysUntilRenewal { get; set; }
    }

    public class CertificateVerificationSummary
    {
        public string? VerificationCode { get; set; }
        public DateTime? LastVerified { get; set; }
        public string? VerificationUrl { get; set; }
    }
}
