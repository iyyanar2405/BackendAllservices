// GraphQL/Middleware/GraphQLErrorFilter.cs
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Types;
using FluentValidation;

namespace FindingsAPI.Gateway.GraphQL.Middleware
{
    public class GraphQLErrorFilter : IErrorFilter
    {
        private readonly ILogger<GraphQLErrorFilter> _logger;

        public GraphQLErrorFilter(ILogger<GraphQLErrorFilter> logger)
        {
            _logger = logger;
        }

        public IError OnError(IError error)
        {
            // Log the error
            _logger.LogError("GraphQL error: {Message} - Path: {Path}", 
                error.Exception?.Message ?? error.Message,
                error.Path);
            
            // Hide internal exceptions in production
            if (error.Exception != null)
            {
                return error;
            }
            
            // Customize validation errors
            if (error.Exception is ValidationException validationException)
            {
                return error;
            }
            
            return error;
        }
    }
}