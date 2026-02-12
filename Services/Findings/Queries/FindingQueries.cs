// Queries/FindingQueries.cs
namespace FindingsAPI.Gateway
{
    public class GetFindingsQuery
    {
        public int? CompanyId { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public bool IncludeCompany { get; set; }
        public bool IncludeSite { get; set; }
    }

    public class SearchFindingsQuery
    {
        public string SearchTerm { get; set; }
        public SearchField SearchIn { get; set; }
        public bool IncludeCompany { get; set; }
    }

    public enum SearchField
    {
        Title,
        Description,
        Number,
        All
    }
}