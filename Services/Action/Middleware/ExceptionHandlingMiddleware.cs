using System.Net;
using System.Text.Json;

namespace ActionService.Middleware
{
    /// <summary>
    /// Middleware to handle exceptions and convert them to appropriate HTTP responses
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred while executing the request.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new { message = exception.Message, type = exception.GetType().Name };

            switch (exception)
            {
                case System.Collections.Generic.KeyNotFoundException:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    break;
                case ArgumentException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    break;
                case UnauthorizedAccessException:
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    break;
                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    break;
            }

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
