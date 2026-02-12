namespace CertificateService.Models
{
    public class CertificateListResponse
    {
        public int CertificateId { get; set; }
        public string? CertificateNumber { get; set; }
        public int CompanyId { get; set; }
        public List<int> ServiceIds { get; set; } = new();
        public List<int> SiteIds { get; set; } = new();
        public string? Status { get; set; }
        public DateTime? IssuedDate { get; set; }
        public DateTime? ValidUntil { get; set; }
        public string? RevisionNumber { get; set; }
    }
}
