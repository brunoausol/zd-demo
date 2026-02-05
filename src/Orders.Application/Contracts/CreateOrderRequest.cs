namespace Orders.Application.Contracts;

public sealed record CreateOrderRequest(
    string FirstName,
    string LastName,
    decimal TotalAmount);
