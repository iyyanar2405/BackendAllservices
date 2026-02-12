using HotChocolate;

namespace FindingsAPI.Gateway.Models.Actions
{
    [GraphQLName("BaseGraphResponse")]
    public class BaseGraphResponse<T>
    {
        public T? Data { get; set; }
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public string? ErrorCode { get; set; }
    }
}
