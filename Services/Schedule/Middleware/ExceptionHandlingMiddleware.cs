using System.Text.Json;

namespace ScheduleService.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger) { _next = next; _logger = logger; }
        public async Task InvokeAsync(HttpContext context) { try { await _next(context); } catch (Exception ex) { _logger.LogError(ex, "Exception"); await HandleExceptionAsync(context, ex); } }
        private static Task HandleExceptionAsync(HttpContext context, Exception ex) { context.Response.ContentType = "application/json"; context.Response.StatusCode = ex is System.Collections.Generic.KeyNotFoundException ? 404 : 500; return context.Response.WriteAsJsonAsync(new { error = ex.Message }); }
    }
}
