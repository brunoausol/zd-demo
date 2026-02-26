using Orders.Abp.Application.Contracts.Orders;

namespace Orders.Abp.Application.Messaging;

public interface IPurchaseSurveySender
{
    Task SendAsync(OrderSurveyRequest request, CancellationToken cancellationToken = default);
}
