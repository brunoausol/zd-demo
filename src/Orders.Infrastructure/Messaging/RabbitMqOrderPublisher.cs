using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orders.Application.Abstractions;
using Orders.Application.Contracts;
using Orders.Infrastructure.Options;
using RabbitMQ.Client;

namespace Orders.Infrastructure.Messaging;

public sealed class RabbitMqOrderPublisher : IOrderPublisher, IDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqOrderPublisher> _logger;
    private readonly IConnection _connection;

    public RabbitMqOrderPublisher(
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqOrderPublisher> logger)
    {
        _options = options.Value;
        _logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Port,
            UserName = _options.Username,
            Password = _options.Password,
            AutomaticRecoveryEnabled = true
        };

        _connection = factory.CreateConnection("orders-api-publisher");
        using var setupChannel = _connection.CreateModel();
        setupChannel.ExchangeDeclare(_options.Exchange, ExchangeType.Topic, durable: true, autoDelete: false);
        setupChannel.ExchangeDeclare(_options.DeadLetterExchange, ExchangeType.Topic, durable: true, autoDelete: false);
        setupChannel.QueueDeclare(
            queue: _options.Queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object>
            {
                ["x-dead-letter-exchange"] = _options.DeadLetterExchange,
                ["x-dead-letter-routing-key"] = _options.DeadLetterRoutingKey
            });
        setupChannel.QueueBind(
            queue: _options.Queue,
            exchange: _options.Exchange,
            routingKey: _options.RoutingKey);
        setupChannel.QueueDeclare(
            queue: _options.DeadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        setupChannel.QueueBind(
            queue: _options.DeadLetterQueue,
            exchange: _options.DeadLetterExchange,
            routingKey: _options.DeadLetterRoutingKey);
    }

    public Task PublishAsync(OrderCreatedEvent message, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var payload = JsonSerializer.SerializeToUtf8Bytes(message);
        using var channel = _connection.CreateModel();
        channel.ConfirmSelect();

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";
        properties.Type = "order.created";
        properties.MessageId = message.Id.ToString();

        channel.BasicPublish(
            exchange: _options.Exchange,
            routingKey: _options.RoutingKey,
            basicProperties: properties,
            body: payload);

        var published = channel.WaitForConfirms(TimeSpan.FromSeconds(_options.PublishConfirmTimeoutSeconds));
        if (!published)
        {
            throw new InvalidOperationException($"RabbitMQ publish confirm timeout for order {message.Id}.");
        }

        _logger.LogInformation("Order created event published to RabbitMQ. OrderId: {OrderId}", message.Id);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _connection.Close();
    }
}
