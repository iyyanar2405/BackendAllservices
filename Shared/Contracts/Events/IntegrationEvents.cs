namespace Shared.Contracts;

public record IntegrationEvent(Guid EventId, DateTime CreatedAt);

public record ProductCreatedIntegrationEvent(Guid EventId, DateTime CreatedAt, int ProductId, string ProductName, decimal Price)
    : IntegrationEvent(EventId, CreatedAt);

public record ProductUpdatedIntegrationEvent(Guid EventId, DateTime CreatedAt, int ProductId, string ProductName, decimal Price)
    : IntegrationEvent(EventId, CreatedAt);

public record ProductDeletedIntegrationEvent(Guid EventId, DateTime CreatedAt, int ProductId)
    : IntegrationEvent(EventId, CreatedAt);
