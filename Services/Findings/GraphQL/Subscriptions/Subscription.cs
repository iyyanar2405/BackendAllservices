// GraphQL/Subscriptions/Subscription.cs
using HotChocolate.Subscriptions;
using HotChocolate.Authorization;

namespace FindingsAPI.Gateway.GraphQL
{
    [ExtendObjectType("Subscription")]
    public class Subscription
    {
        [Subscribe]
        [Topic("FindingCreated")]
        [Authorize("CanViewFindings")]
        public FindingCreatedEvent FindingCreated([EventMessage] FindingCreatedEvent message)
        {
            return message;
        }

        [Subscribe]
        [Topic("FindingUpdated")]
        [Authorize("CanViewFindings")]
        public FindingUpdatedEvent FindingUpdated([EventMessage] FindingUpdatedEvent message)
        {
            return message;
        }
    }

    public class FindingCreatedEvent
    {
        public int FindingId { get; set; }
        public int CompanyId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class FindingUpdatedEvent
    {
        public int FindingId { get; set; }
        public string OldStatus { get; set; }
        public string NewStatus { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}