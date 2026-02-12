using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Shared.EventBus.Interfaces;

namespace Shared.EventBus.Implementations;

public class RabbitMQEventBus : IEventBus, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMQEventBus(IConnectionFactory connectionFactory)
    {
        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public Task PublishAsync<T>(T @event) where T : class
    {
        var exchange = typeof(T).Name;
        _channel.ExchangeDeclare(exchange, ExchangeType.Fanout, durable: true);

        var message = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange, string.Empty, null, body);
        return Task.CompletedTask;
    }

    public Task SubscribeAsync<T>(Func<T, Task> handler) where T : class
    {
        var exchange = typeof(T).Name;
        var queue = $"{exchange}.queue";

        _channel.ExchangeDeclare(exchange, ExchangeType.Fanout, durable: true);
        _channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(queue, exchange, string.Empty);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var @event = JsonSerializer.Deserialize<T>(message);

            Task.Run(async () =>
            {
                if (@event != null)
                {
                    await handler(@event);
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            });
        };

        _channel.BasicConsume(queue, false, consumer);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
