namespace Shared.EventBus.Interfaces;

public interface IEventBus
{
    Task PublishAsync<T>(T @event) where T : class;
    Task SubscribeAsync<T>(Func<T, Task> handler) where T : class;
}

public class IntegrationEvent
{
    public Guid EventId { get; set; }
    public DateTime CreatedAt { get; set; }
}
