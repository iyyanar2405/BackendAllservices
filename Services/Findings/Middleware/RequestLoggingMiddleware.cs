// Middleware/RequestLoggingMiddleware.cs
namespace FindingsAPI.Gateway.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var start = DateTime.UtcNow;
            
            _logger.LogInformation("Request started: {Method} {Path}", 
                context.Request.Method, context.Request.Path);
            
            await _next(context);
            
            var duration = DateTime.UtcNow - start;
            
            _logger.LogInformation("Request completed: {Method} {Path} - Status: {StatusCode}, Duration: {Duration}ms",
                context.Request.Method, context.Request.Path, context.Response.StatusCode, duration.TotalMilliseconds);
        }
    }
}