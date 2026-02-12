// Middleware/CorrelationIdMiddleware.cs
using FindingsAPI.Gateway.Services;

namespace FindingsAPI.Gateway.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;

        public CorrelationIdMiddleware(
            RequestDelegate next, 
            ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ICorrelationIdProvider correlationIdProvider)
        {
            var correlationId = GetOrCreateCorrelationId(context, correlationIdProvider);
            
            // Add to response headers
            context.Response.Headers["X-Correlation-Id"] = correlationId;
            
            // Add to logger scope
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["RequestPath"] = context.Request.Path,
                ["UserId"] = context.User?.Identity?.Name ?? "anonymous"
            }))
            {
                await _next(context);
            }
        }

        private string GetOrCreateCorrelationId(HttpContext context, ICorrelationIdProvider provider)
        {
            // Try to get from request header
            if (context.Request.Headers.TryGetValue("X-Correlation-Id", out var headerValue) &&
                !string.IsNullOrEmpty(headerValue))
            {
                return headerValue;
            }
            
            // Generate new correlation ID
            return provider.GenerateCorrelationId();
        }
    }
    
    public class CorrelationIdHandler : DelegatingHandler
    {
        private readonly ICorrelationIdProvider _correlationIdProvider;

        public CorrelationIdHandler(ICorrelationIdProvider correlationIdProvider)
        {
            _correlationIdProvider = correlationIdProvider;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            var correlationId = _correlationIdProvider.GetCorrelationId();
            
            if (!string.IsNullOrEmpty(correlationId))
            {
                request.Headers.Add("X-Correlation-Id", correlationId);
            }
            
            return base.SendAsync(request, cancellationToken);
        }
    }
}