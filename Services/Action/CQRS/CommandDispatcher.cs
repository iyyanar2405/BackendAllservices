namespace ActionService.CQRS
{
    /// <summary>
    /// Base interface for commands
    /// </summary>
    public interface ICommand<TResponse>
    {
    }

    /// <summary>
    /// Base interface for command handlers
    /// </summary>
    public interface ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Command dispatcher interface
    /// </summary>
    public interface ICommandDispatcher
    {
        Task<TResponse> DispatchAsync<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand<TResponse>;
    }

    /// <summary>
    /// Implementation of command dispatcher using dependency injection
    /// </summary>
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CommandDispatcher> _logger;

        public CommandDispatcher(IServiceProvider serviceProvider, ILogger<CommandDispatcher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task<TResponse> DispatchAsync<TCommand, TResponse>(
            TCommand command,
            CancellationToken cancellationToken = default)
            where TCommand : ICommand<TResponse>
        {
            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(typeof(TCommand), typeof(TResponse));
            var handler = _serviceProvider.GetService(handlerType);

            if (handler == null)
            {
                _logger.LogError("No handler found for command type: {CommandType}", typeof(TCommand).Name);
                throw new InvalidOperationException($"No handler found for command type: {typeof(TCommand).Name}");
            }

            var method = handlerType.GetMethod("HandleAsync");
            if (method == null)
            {
                throw new InvalidOperationException($"Handler for {typeof(TCommand).Name} does not have HandleAsync method");
            }

            _logger.LogInformation("Executing command: {CommandType}", typeof(TCommand).Name);

            var result = method.Invoke(handler, new object?[] { command, cancellationToken });
            if (result is Task<TResponse> task)
            {
                return await task;
            }

            throw new InvalidOperationException($"Handler for {typeof(TCommand).Name} returned invalid result type");
        }
    }
}
