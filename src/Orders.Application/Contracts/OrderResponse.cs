namespace Orders.Application.Contracts;

public sealed record OrderResponse(
    Guid Id,
    string CustomerName,
    decimal TotalAmount,
    DateTime CreatedAtUtc);