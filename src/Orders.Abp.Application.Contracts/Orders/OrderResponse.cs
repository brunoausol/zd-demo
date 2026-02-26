namespace Orders.Abp.Application.Contracts.Orders;

public record OrderResponse(
    Guid Id,
    string CustomerName,
    decimal TotalAmount,
    DateTime CreatedAtUtc);
