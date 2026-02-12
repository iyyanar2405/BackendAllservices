namespace AuditService.CQRS
{
    public interface IQuery<TResponse> { }
    public interface IQueryHandler<TQuery, TResponse> where TQuery : IQuery<TResponse>
    {
        Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
    }

    public interface IQueryDispatcher
    {
        Task<TResponse> DispatchAsync<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken = default)
            where TQuery : IQuery<TResponse>;
    }

    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<QueryDispatcher> _logger;

        public QueryDispatcher(IServiceProvider serviceProvider, ILogger<QueryDispatcher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task<TResponse> DispatchAsync<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken = default)
            where TQuery : IQuery<TResponse>
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(typeof(TQuery), typeof(TResponse));
            var handler = _serviceProvider.GetService(handlerType);
            if (handler == null) throw new InvalidOperationException($"No handler found for query type: {typeof(TQuery).Name}");
            var method = handlerType.GetMethod("HandleAsync");
            if (method == null) throw new InvalidOperationException($"Handler missing HandleAsync method");
            var result = method.Invoke(handler, new object?[] { query, cancellationToken });
            if (result is Task<TResponse> task) return await task;
            throw new InvalidOperationException($"Handler returned invalid result type");
        }
    }

    public interface ICommand<TResponse> { }
    public interface ICommandHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
    {
        Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
    }

    public interface ICommandDispatcher
    {
        Task<TResponse> DispatchAsync<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand<TResponse>;
    }

    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CommandDispatcher> _logger;

        public CommandDispatcher(IServiceProvider serviceProvider, ILogger<CommandDispatcher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task<TResponse> DispatchAsync<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand<TResponse>
        {
            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(typeof(TCommand), typeof(TResponse));
            var handler = _serviceProvider.GetService(handlerType);
            if (handler == null) throw new InvalidOperationException($"No handler found for command type: {typeof(TCommand).Name}");
            var method = handlerType.GetMethod("HandleAsync");
            if (method == null) throw new InvalidOperationException($"Handler missing HandleAsync method");
            _logger.LogInformation("Executing command: {CommandType}", typeof(TCommand).Name);
            var result = method.Invoke(handler, new object?[] { command, cancellationToken });
            if (result is Task<TResponse> task) return await task;
            throw new InvalidOperationException($"Handler returned invalid result type");
        }
    }
}
