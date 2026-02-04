namespace Orders.Application.Contracts;

public sealed record OrderCreatedEvent(
    Guid Id,
    decimal TotalAmount,
    DateTime CreatedAtUtc);