namespace Orders.Application.Contracts;

public sealed record OrderResponseV2(
    Guid Id,
    string CustomerName,
    string? CustomerEmail,
    decimal TotalAmount,
    string Currency,
    DateTime CreatedAtUtc);
