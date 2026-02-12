// Models/Finding.cs
namespace FindingsAPI.Gateway
{
    public class Finding
    {
        public int FindingsId { get; set; }
        public string FindingNumber { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public string Response { get; set; }
        public string Description { get; set; }
        public string Severity { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string ClosureNotes { get; set; }
        public string ClosedBy { get; set; }
        public int CompanyId { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public int? SiteId { get; set; }
        public List<int> Services { get; set; }
        public Company Company { get; set; }
        public Site Site { get; set; }
    }
}