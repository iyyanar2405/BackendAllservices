using System.Net;
using System.Text.Json;

namespace CertificateService.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger) { _next = next; _logger = logger; }
        public async Task InvokeAsync(HttpContext context)
        {
            try { await _next(context); }
            catch (Exception ex) { _logger.LogError(ex, "Exception occurred"); await HandleExceptionAsync(context, ex); }
        }
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception switch
            {
                System.Collections.Generic.KeyNotFoundException => StatusCodes.Status404NotFound,
                ArgumentException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };
            var response = new { message = exception.Message, type = exception.GetType().Name };
            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
