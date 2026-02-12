namespace AuditService.Messaging
{
    public interface IEvent { string EventType { get; } DateTime Timestamp { get; } string CorrelationId { get; } }
    public abstract class Event : IEvent
    {
        public string EventType => GetType().Name;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
    }

    public interface IEventPublisher
    {
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent;
    }

    public class InMemoryEventPublisher : IEventPublisher
    {
        private readonly ILogger<InMemoryEventPublisher> _logger;

        public InMemoryEventPublisher(ILogger<InMemoryEventPublisher> logger) => _logger = logger;

        public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
        {
            _logger.LogInformation("Publishing event: {EventType}", typeof(TEvent).Name);
            return Task.CompletedTask;
        }
    }

    public interface IEventService
    {
        Task RaiseEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent;
    }

    public class EventService : IEventService
    {
        private readonly IEventPublisher _eventPublisher;
        public EventService(IEventPublisher eventPublisher) => _eventPublisher = eventPublisher;
        public async Task RaiseEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
            => await _eventPublisher.PublishAsync(@event, cancellationToken);
    }
}
