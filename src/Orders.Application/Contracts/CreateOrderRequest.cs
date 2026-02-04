namespace Orders.Application.Contracts;

public sealed record CreateOrderRequest(
    string CustomerName,
    decimal TotalAmount);
