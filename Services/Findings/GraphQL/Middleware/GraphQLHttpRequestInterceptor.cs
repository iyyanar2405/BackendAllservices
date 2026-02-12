// GraphQL/Middleware/GraphQLHttpRequestInterceptor.cs
using FindingsAPI.Gateway.Services;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Subscriptions;
using HotChocolate.Execution;

namespace FindingsAPI.Gateway.GraphQL.Middleware
{
    public class GraphQLHttpRequestInterceptor : DefaultHttpRequestInterceptor
    {
        private readonly ICorrelationIdProvider _correlationIdProvider;
        private readonly ILogger<GraphQLHttpRequestInterceptor> _logger;

        public GraphQLHttpRequestInterceptor(
            ICorrelationIdProvider correlationIdProvider,
            ILogger<GraphQLHttpRequestInterceptor> logger)
        {
            _correlationIdProvider = correlationIdProvider;
            _logger = logger;
        }

        // public override ValueTask OnCreateAsync(
        //     HttpContext context,
        //     IRequestExecutor requestExecutor,
        //     object requestBuilder, // IQueryRequestBuilder requestBuilder,
        //     CancellationToken cancellationToken)
        // {
        //     // Add correlation ID to GraphQL context
        //     var correlationId = _correlationIdProvider.GetCorrelationId(context);
        //     // requestBuilder.SetProperty("CorrelationId", correlationId);
        //     
        //     // Add user info
        //     if (context.User.Identity?.IsAuthenticated == true)
        //     {
        //         // requestBuilder.SetProperty("UserId", 
        //         //     context.User.FindFirstValue(ClaimTypes.NameIdentifier));
        //         // requestBuilder.SetProperty("UserRoles", 
        //         //     context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList());
        //     }
        //     
        //     _logger.LogInformation("GraphQL request started: {OperationName} by {User}", 
        //         null, context.User.Identity?.Name);
        //     
        //     return base.OnCreateAsync(context, requestExecutor, requestBuilder, cancellationToken);
        // }
    }
}