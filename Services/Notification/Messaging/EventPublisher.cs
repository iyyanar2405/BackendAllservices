namespace NotificationService.Messaging
{
    public interface IEvent { string EventType { get; } DateTime Timestamp { get; } }
    public abstract class Event : IEvent { public string EventType => GetType().Name; public DateTime Timestamp { get; } = DateTime.UtcNow; }
    public interface IEventPublisher { Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent; }
    public class InMemoryEventPublisher : IEventPublisher { private readonly ILogger<InMemoryEventPublisher> _logger; public InMemoryEventPublisher(ILogger<InMemoryEventPublisher> logger) => _logger = logger; public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent { _logger.LogInformation("Publishing: {Type}", typeof(TEvent).Name); return Task.CompletedTask; } }
    public interface IEventService { Task RaiseEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent; }
    public class EventService : IEventService { private readonly IEventPublisher _pub; public EventService(IEventPublisher pub) => _pub = pub; public async Task RaiseEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent => await _pub.PublishAsync(@event, cancellationToken); }
}
