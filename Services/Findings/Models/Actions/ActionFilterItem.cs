using HotChocolate;

namespace FindingsAPI.Gateway.Models.Actions
{
    [GraphQLName("ActionFilterItem")]
    public class ActionFilterItem
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
    }
}
