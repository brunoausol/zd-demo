using Microsoft.Extensions.Logging;
using Orders.Application.Contracts;

namespace Orders.Infrastructure.Messaging;

public sealed class PurchaseSurveySender : IPurchaseSurveySender
{
    private readonly ILogger<PurchaseSurveySender> _logger;

    public PurchaseSurveySender(ILogger<PurchaseSurveySender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(OrderSurveyRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Purchase survey sent. OrderId: {OrderId}, Customer: {CustomerName}, PurchasedAtUtc: {PurchasedAtUtc}",
            request.OrderId,
            request.CustomerName,
            request.PurchasedAtUtc);

        return Task.CompletedTask;
    }
}
