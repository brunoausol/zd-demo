namespace Orders.Abp.Application.Contracts.Integration;

public record OrderCreatedEto(
    Guid Id,
    string CustomerName,
    decimal TotalAmount,
    DateTime CreatedAtUtc);
