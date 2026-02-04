namespace Orders.Application.Contracts;

public sealed record CreateOrderRequestV1(
    string CustomerName,
    decimal TotalAmount,
    string? Currency);
