// GraphQL/Mutations/MutationInputs.cs
namespace FindingsAPI.Gateway.GraphQL.Mutations
{
    public class UpdateFindingInput
    {
        [GraphQLType(typeof(NonNullType<IntType>))]
        public int FindingId { get; set; }
        
        public string Status { get; set; }
        public string Response { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class UpdateFindingPayload
    {
        public Finding Finding { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class CloseFindingPayload
    {
        public Finding Finding { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class BulkUpdatePayload
    {
        public int UpdatedCount { get; set; }
        public int FailedCount { get; set; }
        public List<int> FailedIds { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}