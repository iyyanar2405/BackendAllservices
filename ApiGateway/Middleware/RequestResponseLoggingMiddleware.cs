namespace ApiGateway.Middleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        _logger.LogInformation("Request: {Method} {Path}", context.Request.Method, context.Request.Path);

        await _next(context);

        stopwatch.Stop();
        _logger.LogInformation("Response: {StatusCode} ({ElapsedMilliseconds}ms)", 
            context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
    }
}
