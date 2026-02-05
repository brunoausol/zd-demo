namespace Orders.Application.Contracts;

public sealed record CreateOrderRequest(
    string? CustomerName,
    string? FirstName,
    string? LastName,
    decimal TotalAmount);
