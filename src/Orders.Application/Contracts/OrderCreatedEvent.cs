namespace Orders.Application.Contracts;

public sealed record OrderCreatedEvent(
    Guid Id,
    string CustomerName,
    decimal TotalAmount,
    DateTime CreatedAtUtc);
