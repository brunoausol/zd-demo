using Microsoft.Extensions.Logging;
using Orders.Application.Contracts;

namespace Orders.Infrastructure.Messaging.Envelopes;

public sealed class SendPurchaseSurveyV1EnvelopeHandler : IHangfireEnvelopeHandler
{
    private readonly IPurchaseSurveySender _surveySender;
    private readonly ILogger<SendPurchaseSurveyV1EnvelopeHandler> _logger;

    public SendPurchaseSurveyV1EnvelopeHandler(
        IPurchaseSurveySender surveySender,
        ILogger<SendPurchaseSurveyV1EnvelopeHandler> logger)
    {
        _surveySender = surveySender;
        _logger = logger;
    }

    public string MessageType => HangfireMessageTypes.SendPurchaseSurveyV1;

    public async Task HandleAsync(HangfireEnvelope envelope, CancellationToken cancellationToken)
    {
        var request = envelope.DeserializePayload<OrderSurveyRequest>();
        _logger.LogInformation("Executing purchase survey handler v1 for order {OrderId}.", request.OrderId);
        await _surveySender.SendAsync(request, cancellationToken);
    }
}
