using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orders.Application.Contracts;
using Orders.Infrastructure.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Orders.Infrastructure.Messaging;

public sealed class RabbitMqOrderCreatedConsumer : BackgroundService
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqOrderCreatedConsumer> _logger;
    private readonly IOrderConfirmationEmailSender _emailSender;
    private IConnection? _connection;
    private IModel? _channel;
    private string? _consumerTag;
    private readonly TaskCompletionSource _drainedTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private int _messagesInFlight;
    private volatile bool _isStopping;

    public RabbitMqOrderCreatedConsumer(
        IOptions<RabbitMqOptions> options,
        IOrderConfirmationEmailSender emailSender,
        ILogger<RabbitMqOrderCreatedConsumer> logger)
    {
        _options = options.Value;
        _emailSender = emailSender;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Port,
            UserName = _options.Username,
            Password = _options.Password,
            AutomaticRecoveryEnabled = true,
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection("orders-api-consumer");
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(_options.Exchange, ExchangeType.Topic, durable: true, autoDelete: false);
        _channel.ExchangeDeclare(_options.DeadLetterExchange, ExchangeType.Topic, durable: true, autoDelete: false);
        _channel.QueueDeclare(
            _options.Queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object>
            {
                ["x-dead-letter-exchange"] = _options.DeadLetterExchange,
                ["x-dead-letter-routing-key"] = _options.DeadLetterRoutingKey
            });
        _channel.QueueBind(_options.Queue, _options.Exchange, _options.RoutingKey);
        _channel.QueueDeclare(_options.DeadLetterQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueBind(_options.DeadLetterQueue, _options.DeadLetterExchange, _options.DeadLetterRoutingKey);
        _channel.BasicQos(0, 1, false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (_, eventArgs) =>
        {
            if (_isStopping)
            {
                _channel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: true);
                return;
            }

            Interlocked.Increment(ref _messagesInFlight);
            try
            {
                var json = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var message = JsonSerializer.Deserialize<OrderCreatedEvent>(json);
                if (message is null)
                {
                    throw new InvalidOperationException("OrderCreatedEvent payload is null.");
                }

                await ProcessWithRetryAsync(message, stoppingToken);
                _channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process order.created event.");
                _channel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: false);
            }
            finally
            {
                var remaining = Interlocked.Decrement(ref _messagesInFlight);
                if (_isStopping && remaining == 0)
                {
                    _drainedTcs.TrySetResult();
                }
            }
        };

        _consumerTag = _channel.BasicConsume(_options.Queue, autoAck: false, consumer: consumer);

        _logger.LogInformation("RabbitMQ consumer started for queue {Queue}.", _options.Queue);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (TaskCanceledException)
        {
            // Expected on shutdown.
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _isStopping = true;

        if (!string.IsNullOrWhiteSpace(_consumerTag) && _channel is not null && _channel.IsOpen)
        {
            _channel.BasicCancel(_consumerTag);
        }

        if (Volatile.Read(ref _messagesInFlight) == 0)
        {
            _drainedTcs.TrySetResult();
        }

        return StopAndDrainAsync(cancellationToken);
    }

    private async Task StopAndDrainAsync(CancellationToken cancellationToken)
    {
        var drainTimeout = _options.ShutdownDrainTimeoutSeconds <= 0
            ? TimeSpan.FromSeconds(30)
            : TimeSpan.FromSeconds(_options.ShutdownDrainTimeoutSeconds);

        try
        {
            await _drainedTcs.Task.WaitAsync(drainTimeout, cancellationToken);
        }
        catch (TimeoutException)
        {
            _logger.LogWarning(
                "RabbitMQ consumer shutdown timed out after {TimeoutSeconds}s with {InFlight} message(s) still running.",
                drainTimeout.TotalSeconds,
                Volatile.Read(ref _messagesInFlight));
        }

        await base.StopAsync(cancellationToken);
    }

    private async Task ProcessWithRetryAsync(OrderCreatedEvent message, CancellationToken cancellationToken)
    {
        var maxAttempts = _options.ProcessingRetryCount <= 0 ? 1 : _options.ProcessingRetryCount;
        Exception? lastException = null;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await _emailSender.SendAsync(message, cancellationToken);
                return;
            }
            catch (Exception ex)
            {
                lastException = ex;
                _logger.LogWarning(
                    ex,
                    "Attempt {Attempt}/{MaxAttempts} failed for order confirmation e-mail. OrderId: {OrderId}",
                    attempt,
                    maxAttempts,
                    message.Id);
            }
        }

        throw new InvalidOperationException(
            $"Failed to process order.created for order {message.Id} after {maxAttempts} attempts.",
            lastException);
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
