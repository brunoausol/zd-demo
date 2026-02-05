namespace Orders.Application.Contracts;

public sealed record OrderResponse(
    Guid Id,
    string CustomerName,
    string FirstName,
    string LastName,
    decimal TotalAmount,
    DateTime CreatedAtUtc);