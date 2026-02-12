namespace CertificateService.Models.Rest
{
    public class CertificateSearchRequest
    {
        public string? SearchTerm { get; set; }
        public List<int> CompanyIds { get; set; } = new();
        public List<int> SiteIds { get; set; } = new();
        public List<int> ServiceIds { get; set; } = new();
        public List<string> Statuses { get; set; } = new();
        public List<string> Standards { get; set; } = new();
        public List<string> Countries { get; set; } = new();
        public List<string> AccreditationBodies { get; set; } = new();
        public List<string> CertificateTypes { get; set; } = new();
        public ExpiryPeriod? ExpiryPeriod { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }
    }

    public class ExpiryPeriod
    {
        public int? WithinDays { get; set; }
    }
}
