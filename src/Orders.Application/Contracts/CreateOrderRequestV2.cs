namespace Orders.Application.Contracts;

public sealed record CreateOrderRequestV2(
    string CustomerName,
    string? CustomerEmail,
    decimal TotalAmount,
    string? Currency);
