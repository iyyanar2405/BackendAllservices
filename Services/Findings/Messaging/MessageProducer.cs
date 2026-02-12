// Messaging/MessageProducer.cs
using System.Text.Json;

namespace FindingsAPI.Gateway.Services
{
    public interface IMessageProducer
    {
        Task PublishAsync<T>(T message);
    }

    public class MessageProducer : IMessageProducer
    {
        private readonly ILogger<MessageProducer> _logger;

        public MessageProducer(ILogger<MessageProducer> logger)
        {
            _logger = logger;
        }

        public async Task PublishAsync<T>(T message)
        {
            // For now, just log the message
            // In production, this would publish to a message broker like RabbitMQ, Azure Service Bus, etc.
            _logger.LogInformation("Publishing message: {MessageType} - {Message}", 
                typeof(T).Name, JsonSerializer.Serialize(message));
            
            await Task.CompletedTask;
        }
    }
}