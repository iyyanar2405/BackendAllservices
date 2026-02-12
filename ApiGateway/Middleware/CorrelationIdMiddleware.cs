namespace ApiGateway.Middleware;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeaderName = "X-Correlation-Id";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var value)
            ? value.ToString()
            : Guid.NewGuid().ToString();

        context.Items[CorrelationIdHeaderName] = correlationId;
        context.Response.Headers.Add(CorrelationIdHeaderName, correlationId);

        await _next(context);
    }
}
