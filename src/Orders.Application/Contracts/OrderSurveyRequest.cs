namespace Orders.Application.Contracts;

public sealed record OrderSurveyRequest(
    Guid OrderId,
    string CustomerName,
    DateTime PurchasedAtUtc);
