namespace Orders.Application.Contracts;

public sealed record OrderResponseV1(
    Guid Id,
    string CustomerName,
    decimal TotalAmount,
    string Currency,
    DateTime CreatedAtUtc);
