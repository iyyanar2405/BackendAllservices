namespace SettingsService.CQRS
{
    public interface IQuery<TResponse> { }
    public interface IQueryHandler<TQuery, TResponse> where TQuery : IQuery<TResponse> { Task<TResponse> HandleAsync(TQuery query, CancellationToken ct = default); }
    public interface IQueryDispatcher { Task<TResponse> DispatchAsync<TQuery, TResponse>(TQuery query, CancellationToken ct = default) where TQuery : IQuery<TResponse>; }
    public class QueryDispatcher : IQueryDispatcher { private IServiceProvider _sp; public QueryDispatcher(IServiceProvider sp) => _sp = sp; public async Task<TResponse> DispatchAsync<TQuery, TResponse>(TQuery query, CancellationToken ct = default) where TQuery : IQuery<TResponse> { var ht = typeof(IQueryHandler<,>).MakeGenericType(typeof(TQuery), typeof(TResponse)); var h = _sp.GetService(ht); if (h == null) throw new Exception("No handler"); var m = ht.GetMethod("HandleAsync"); var r = m.Invoke(h, new object?[] { query, ct }); if (r is Task<TResponse> t) return await t; throw new Exception("Bad result"); } }
    public interface ICommand<TResponse> { }
    public interface ICommandHandler<TCommand, TResponse> where TCommand : ICommand<TResponse> { Task<TResponse> HandleAsync(TCommand cmd, CancellationToken ct = default); }
    public interface ICommandDispatcher { Task<TResponse> DispatchAsync<TCommand, TResponse>(TCommand cmd, CancellationToken ct = default) where TCommand : ICommand<TResponse>; }
    public class CommandDispatcher : ICommandDispatcher { private IServiceProvider _sp; private ILogger<CommandDispatcher> _logger; public CommandDispatcher(IServiceProvider sp, ILogger<CommandDispatcher> logger) { _sp = sp; _logger = logger; } public async Task<TResponse> DispatchAsync<TCommand, TResponse>(TCommand cmd, CancellationToken ct = default) where TCommand : ICommand<TResponse> { var ht = typeof(ICommandHandler<,>).MakeGenericType(typeof(TCommand), typeof(TResponse)); var h = _sp.GetService(ht); if (h == null) throw new Exception("No handler"); var m = ht.GetMethod("HandleAsync"); var r = m.Invoke(h, new object?[] { cmd, ct }); if (r is Task<TResponse> t) return await t; throw new Exception("Bad result"); } }
}
