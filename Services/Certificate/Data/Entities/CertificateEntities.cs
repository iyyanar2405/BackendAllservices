namespace CertificateService.Data.Entities
{
    public class Certificate
    {
        public int CertificateId { get; set; }
        public string CertificateNumber { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public int? SiteId { get; set; }
        public int ServiceId { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? CertificateType { get; set; }
        public string? Scope { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public int? IssuedBy { get; set; }
        public int RevisionNumber { get; set; }
        public int? PreviousCertificateId { get; set; }
        public string? CertificatePath { get; set; }
        public int? AuditId { get; set; }
        public string? Notes { get; set; }
    }

    public class Company
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public int? CountryId { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactEmail { get; set; }
    }

    public class Site
    {
        public int SiteId { get; set; }
        public string SiteName { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public string? Address { get; set; }
        public int? CountryId { get; set; }
    }

    public class Service
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string ServiceCode { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class CertificateService
    {
        public int CertificateServiceId { get; set; }
        public int CertificateId { get; set; }
        public int ServiceId { get; set; }
        public bool IsActive { get; set; }
        public string? Scope { get; set; }
    }

    public class CertificateSite
    {
        public int CertificateSiteId { get; set; }
        public int CertificateId { get; set; }
        public int SiteId { get; set; }
        public bool IsActive { get; set; }
        public string? Scope { get; set; }
    }

    public class CertificateAdditionalScope
    {
        public int CertificateAdditionalScopeId { get; set; }
        public int CertificateId { get; set; }
        public string ScopeDescription { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class Country
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string CountryCodeAlpha2 { get; set; } = string.Empty;
    }

    public class Audit
    {
        public int AuditId { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? LeadAuditor { get; set; }
        public string? Type { get; set; }
    }
}
