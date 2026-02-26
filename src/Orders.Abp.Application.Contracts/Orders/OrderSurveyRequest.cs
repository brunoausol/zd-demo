namespace Orders.Abp.Application.Contracts.Orders;

public record OrderSurveyRequest(
    Guid OrderId,
    string CustomerName,
    DateTime OrderCreatedAtUtc);
