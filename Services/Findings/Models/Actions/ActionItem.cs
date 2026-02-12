using HotChocolate;

namespace FindingsAPI.Gateway.Models.Actions
{
    [GraphQLName("Action")]
    public class ActionItem
    {
        public int Id { get; set; }
        public string? Action { get; set; }
        public DateTime? DueDate { get; set; }
        public int HighPriority { get; set; }
        public string? Message { get; set; }
        public string? Language { get; set; }
        public string? Service { get; set; }
        public string? Site { get; set; }
        public string? EntityType { get; set; }
        public string? EntityId { get; set; }
        public string? Subject { get; set; }
        public string? SnowLink { get; set; }
    }
}
