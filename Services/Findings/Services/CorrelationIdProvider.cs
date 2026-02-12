// Services/CorrelationIdProvider.cs
namespace FindingsAPI.Gateway.Services
{
    public interface ICorrelationIdProvider
    {
        string GenerateCorrelationId();
        string GetCorrelationId();
        string GetCorrelationId(HttpContext context);
    }

    public class CorrelationIdProvider : ICorrelationIdProvider
    {
        private static readonly AsyncLocal<string> _correlationId = new();

        public string GenerateCorrelationId()
        {
            var correlationId = Guid.NewGuid().ToString();
            _correlationId.Value = correlationId;
            return correlationId;
        }

        public string GetCorrelationId()
        {
            return _correlationId.Value ?? GenerateCorrelationId();
        }

        public string GetCorrelationId(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("X-Correlation-Id", out var headerValue) &&
                !string.IsNullOrEmpty(headerValue))
            {
                _correlationId.Value = headerValue;
                return headerValue.ToString();
            }
            
            return GetCorrelationId();
        }
    }
}