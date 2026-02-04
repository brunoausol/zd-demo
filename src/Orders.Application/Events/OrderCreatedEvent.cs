namespace Orders.Application.Events;

public sealed record OrderCreatedEvent(
    Guid Id,
    string CustomerName,
    string? CustomerEmail,
    decimal TotalAmount,
    string Currency,
    DateTime CreatedAtUtc);
