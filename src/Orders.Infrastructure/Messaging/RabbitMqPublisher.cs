using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orders.Application.Abstractions;
using Orders.Application.Contracts;
using Orders.Infrastructure.Options;
using RabbitMQ.Client;

namespace Orders.Infrastructure.Messaging;

public sealed class RabbitMqPublisher : IOrderPublisher, IDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqPublisher> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqPublisher(IOptions<RabbitMqOptions> options, ILogger<RabbitMqPublisher> logger)
    {
        _options = options.Value;
        _logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Port,
            UserName = _options.Username,
            Password = _options.Password
        };

        _connection = factory.CreateConnection("orders-api-publisher");
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(_options.Exchange, ExchangeType.Topic, durable: true, autoDelete: false);
        _channel.QueueDeclare(
            queue: _options.Queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        _channel.QueueBind(
            queue: _options.Queue,
            exchange: _options.Exchange,
            routingKey: _options.RoutingKey);
    }

    public Task PublishAsync(OrderCreatedEvent message, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.SerializeToUtf8Bytes(message);
        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";

        _channel.BasicPublish(
            exchange: _options.Exchange,
            routingKey: _options.RoutingKey,
            basicProperties: properties,
            body: payload);

        _logger.LogInformation("Order published to RabbitMQ with id {OrderId}", message.Id);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
    }
}
