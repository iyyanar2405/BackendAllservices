using System.Text.Json;

namespace ActionService.Messaging
{
    /// <summary>
    /// Base interface for event publishing
    /// </summary>
    public interface IEventPublisher
    {
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
            where TEvent : IEvent;
    }

    /// <summary>
    /// Base interface for all events
    /// </summary>
    public interface IEvent
    {
        string EventType { get; }
        DateTime Timestamp { get; }
        string CorrelationId { get; }
    }

    /// <summary>
    /// Base event class
    /// </summary>
    public abstract class Event : IEvent
    {
        public string EventType => GetType().AssemblyQualifiedName ?? GetType().Name;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// In-memory event publisher implementation
    /// </summary>
    public class InMemoryEventPublisher : IEventPublisher
    {
        private readonly ILogger<InMemoryEventPublisher> _logger;
        private static readonly Dictionary<string, List<Delegate>> _subscribers = new();

        public InMemoryEventPublisher(ILogger<InMemoryEventPublisher> logger)
        {
            _logger = logger;
        }

        public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
            where TEvent : IEvent
        {
            var eventType = typeof(TEvent).AssemblyQualifiedName ?? typeof(TEvent).Name;
            
            _logger.LogInformation("Publishing event: {EventType} with CorrelationId: {CorrelationId}", 
                eventType, @event.CorrelationId);

            if (_subscribers.TryGetValue(eventType, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    try
                    {
                        if (handler is Func<TEvent, Task> asyncHandler)
                        {
                            asyncHandler(@event).Wait(cancellationToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error executing event handler for event type: {EventType}", eventType);
                    }
                }
            }

            return Task.CompletedTask;
        }

        public static void Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
        {
            var eventType = typeof(TEvent).AssemblyQualifiedName ?? typeof(TEvent).Name;
            
            if (!_subscribers.ContainsKey(eventType))
            {
                _subscribers[eventType] = new List<Delegate>();
            }

            _subscribers[eventType].Add(handler);
        }
    }

    /// <summary>
    /// Event subscriber interface
    /// </summary>
    public interface IEventSubscriber
    {
        Task HandleAsync(IEvent @event);
    }

    /// <summary>
    /// Generic event handler interface
    /// </summary>
    public interface IEventHandler<TEvent> where TEvent : IEvent
    {
        Task HandleAsync(TEvent @event);
    }

    /// <summary>
    /// RabbitMQ-based event publisher (placeholder for future implementation)
    /// </summary>
    public interface IRabbitMQEventPublisher : IEventPublisher
    {
    }

    /// <summary>
    /// Event service for publishing domain events
    /// </summary>
    public interface IEventService
    {
        Task RaiseEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
            where TEvent : IEvent;
    }

    public class EventService : IEventService
    {
        private readonly IEventPublisher _eventPublisher;

        public EventService(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public async Task RaiseEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
            where TEvent : IEvent
        {
            await _eventPublisher.PublishAsync(@event, cancellationToken);
        }
    }
}
