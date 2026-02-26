using Microsoft.Extensions.Logging;
using Orders.Abp.Application.Contracts.Orders;
using Volo.Abp.DependencyInjection;

namespace Orders.Abp.Application.Messaging;

public class PurchaseSurveySender : IPurchaseSurveySender, ITransientDependency
{
    private readonly ILogger<PurchaseSurveySender> _logger;

    public PurchaseSurveySender(ILogger<PurchaseSurveySender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(OrderSurveyRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[SURVEY] Purchase survey sent for OrderId={OrderId}, Customer={CustomerName}.",
            request.OrderId,
            request.CustomerName);

        return Task.CompletedTask;
    }
}
