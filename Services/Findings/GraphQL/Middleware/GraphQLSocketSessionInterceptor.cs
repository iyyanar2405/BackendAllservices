// GraphQL/Middleware/GraphQLSocketSessionInterceptor.cs
using HotChocolate.AspNetCore.Subscriptions;

namespace FindingsAPI.Gateway.GraphQL.Middleware
{
    public class GraphQLSocketSessionInterceptor // : DefaultSocketSessionInterceptor
    {
        private readonly ILogger<GraphQLSocketSessionInterceptor> _logger;

        public GraphQLSocketSessionInterceptor(ILogger<GraphQLSocketSessionInterceptor> logger)
        {
            _logger = logger;
        }

        // public override ValueTask<ConnectionStatus> OnConnectAsync(
        //     ISocketConnection connection, 
        //     InitializeConnectionMessage message,
        //     CancellationToken cancellationToken)
        // {
        //     _logger.LogInformation("GraphQL WebSocket connected: {ConnectionId}", 
        //         connection.Id);
        //     
        //     return base.OnConnectAsync(connection, message, cancellationToken);
        // }

        // public override ValueTask OnRequestAsync(
        //     ISocketConnection connection,
        //     IQueryRequestBuilder requestBuilder,
        //     CancellationToken cancellationToken)
        // {
        //     // Add WebSocket-specific properties
        //     requestBuilder.SetProperty("WebSocketConnectionId", connection.Id);
        //     
        //     return base.OnRequestAsync(connection, requestBuilder, cancellationToken);
        // }
    }
}