namespace CertificateService.Models.Rest
{
    public class CertificateListPageData
    {
        public List<CertificateListItemResponse> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public Dictionary<string, int> StatusCounts { get; set; } = new();
        public int ExpiringWithin30Days { get; set; }
        public int ExpiringWithin90Days { get; set; }
    }
}
