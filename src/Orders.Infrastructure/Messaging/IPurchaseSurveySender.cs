using Orders.Application.Contracts;

namespace Orders.Infrastructure.Messaging;

public interface IPurchaseSurveySender
{
    Task SendAsync(OrderSurveyRequest request, CancellationToken cancellationToken);
}
