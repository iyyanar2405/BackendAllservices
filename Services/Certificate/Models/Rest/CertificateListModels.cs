namespace CertificateService.Models.Rest
{
    public class CertificateListRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }
        public CertificateListFilters Filters { get; set; } = new();
    }

    public class CertificateListFilters
    {
        public List<int> CompanyIds { get; set; } = new();
        public List<int> SiteIds { get; set; } = new();
        public List<int> ServiceIds { get; set; } = new();
        public List<string> Statuses { get; set; } = new();
        public DateRange? IssuedDateRange { get; set; }
        public DateRange? ExpiryDateRange { get; set; }
        public List<string> CertificateNumbers { get; set; } = new();
        public bool? IncludeSuspended { get; set; }
        public bool? IncludeExpired { get; set; }
    }

    public class DateRange
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class CertificateListResponse
    {
        public List<CertificateListItemResponse> Certificates { get; set; } = new();
        public CertificateListSummary Summary { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    public class CertificateListSummary
    {
        public int TotalCertificates { get; set; }
        public Dictionary<string, int> ByStatus { get; set; } = new();
        public int ExpiringWithin30Days { get; set; }
        public int ExpiringWithin90Days { get; set; }
    }

    public class CertificateListItemResponse
    {
        public int CertificateId { get; set; }
        public string? CertificateNumber { get; set; }
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public List<int> ServiceIds { get; set; } = new();
        public List<CertificateServiceSummary> Services { get; set; } = new();
        public List<int> SiteIds { get; set; } = new();
        public List<CertificateSiteSummary> Sites { get; set; } = new();
        public string? Status { get; set; }
        public DateTime? IssuedDate { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidUntil { get; set; }
        public string? RevisionNumber { get; set; }
        public string? CertificateType { get; set; }
        public string? AccreditationBody { get; set; }
        public string? Country { get; set; }
        public DateTime? SuspendedDate { get; set; }
        public string? SuspensionReason { get; set; }
        public DateTime? LastAuditDate { get; set; }
        public DateTime? NextAuditDate { get; set; }
        public int? DaysUntilExpiry { get; set; }
        public string? RenewalStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class CertificateServiceSummary
    {
        public int ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public string? Standard { get; set; }
        public string? Scope { get; set; }
    }

    public class CertificateSiteSummary
    {
        public int SiteId { get; set; }
        public string? SiteName { get; set; }
        public string? SiteAddress { get; set; }
    }
}
