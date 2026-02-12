namespace ScheduleService.Messaging
{
    public interface IEvent { string EventType { get; } DateTime Timestamp { get; } }
    public abstract class Event : IEvent { public string EventType => GetType().Name; public DateTime Timestamp => DateTime.UtcNow; }
    public interface IEventPublisher { Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IEvent; }
    public class InMemoryEventPublisher : IEventPublisher { private readonly ILogger<InMemoryEventPublisher> _logger; public InMemoryEventPublisher(ILogger<InMemoryEventPublisher> logger) => _logger = logger; public Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IEvent { _logger.LogInformation("Event: {Type}", typeof(T).Name); return Task.CompletedTask; } }
}
