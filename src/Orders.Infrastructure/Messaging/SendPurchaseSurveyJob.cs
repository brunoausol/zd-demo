using Hangfire;
using Microsoft.Extensions.Logging;
using Orders.Application.Contracts;

namespace Orders.Infrastructure.Messaging;

public sealed class SendPurchaseSurveyJob
{
    private readonly IPurchaseSurveySender _surveySender;
    private readonly ILogger<SendPurchaseSurveyJob> _logger;

    public SendPurchaseSurveyJob(
        IPurchaseSurveySender surveySender,
        ILogger<SendPurchaseSurveyJob> logger)
    {
        _surveySender = surveySender;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task ExecuteAsync(OrderSurveyRequest request)
    {
        _logger.LogInformation("Executing purchase survey job for order {OrderId}.", request.OrderId);
        await _surveySender.SendAsync(request, CancellationToken.None);
    }
}
