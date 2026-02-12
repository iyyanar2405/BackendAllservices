// Commands/FindingCommands.cs
namespace FindingsAPI.Gateway
{
    public class CreateFindingCommand
    {
        public string Title { get; set; }
        public string Category { get; set; }
        public int CompanyId { get; set; }
        public int? SiteId { get; set; }
        public List<int> Services { get; set; }
        public string Description { get; set; }
        public string Severity { get; set; }
        public string CreatedBy { get; set; }
    }

    public class UpdateFindingCommand
    {
        public int FindingId { get; set; }
        public string Status { get; set; }
        public string Response { get; set; }
        public DateTime? DueDate { get; set; }
        public string UpdatedBy { get; set; }
    }

    public class CloseFindingCommand
    {
        public int FindingId { get; set; }
        public string ClosureNotes { get; set; }
        public string ClosedBy { get; set; }
    }

    public class BulkUpdateStatusCommand
    {
        public List<int> FindingIds { get; set; }
        public string NewStatus { get; set; }
        public string Reason { get; set; }
        public string UpdatedBy { get; set; }
    }

    public class BulkUpdateResult
    {
        public int UpdatedCount { get; set; }
        public int FailedCount { get; set; }
        public List<int> FailedIds { get; set; }
    }
}