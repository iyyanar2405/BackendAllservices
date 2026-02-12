namespace ActionService.CQRS
{
    /// <summary>
    /// Base interface for queries
    /// </summary>
    public interface IQuery<TResponse>
    {
    }

    /// <summary>
    /// Base interface for query handlers
    /// </summary>
    public interface IQueryHandler<TQuery, TResponse> 
        where TQuery : IQuery<TResponse>
    {
        Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Query dispatcher interface
    /// </summary>
    public interface IQueryDispatcher
    {
        Task<TResponse> DispatchAsync<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken = default)
            where TQuery : IQuery<TResponse>;
    }

    /// <summary>
    /// Implementation of query dispatcher using dependency injection
    /// </summary>
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<QueryDispatcher> _logger;

        public QueryDispatcher(IServiceProvider serviceProvider, ILogger<QueryDispatcher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task<TResponse> DispatchAsync<TQuery, TResponse>(
            TQuery query,
            CancellationToken cancellationToken = default)
            where TQuery : IQuery<TResponse>
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(typeof(TQuery), typeof(TResponse));
            var handler = _serviceProvider.GetService(handlerType);

            if (handler == null)
            {
                _logger.LogError("No handler found for query type: {QueryType}", typeof(TQuery).Name);
                throw new InvalidOperationException($"No handler found for query type: {typeof(TQuery).Name}");
            }

            var method = handlerType.GetMethod("HandleAsync");
            if (method == null)
            {
                throw new InvalidOperationException($"Handler for {typeof(TQuery).Name} does not have HandleAsync method");
            }

            var result = method.Invoke(handler, new object?[] { query, cancellationToken });
            if (result is Task<TResponse> task)
            {
                return await task;
            }

            throw new InvalidOperationException($"Handler for {typeof(TQuery).Name} returned invalid result type");
        }
    }
}
