namespace Orders.Infrastructure.Options;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    public string Host { get; init; } = "localhost";
    public int Port { get; init; } = 5672;
    public string Username { get; init; } = "guest";
    public string Password { get; init; } = "guest";
    public string Exchange { get; init; } = "orders.exchange";
    public string RoutingKey { get; init; } = "orders.created";
    public string Queue { get; init; } = "orders.created.queue";
    public string DeadLetterExchange { get; init; } = "orders.deadletter.exchange";
    public string DeadLetterQueue { get; init; } = "orders.created.deadletter.queue";
    public string DeadLetterRoutingKey { get; init; } = "orders.created.deadletter";
    public int PublishConfirmTimeoutSeconds { get; init; } = 5;
    public int ProcessingRetryCount { get; init; } = 3;
    public int ShutdownDrainTimeoutSeconds { get; init; } = 30;
}
