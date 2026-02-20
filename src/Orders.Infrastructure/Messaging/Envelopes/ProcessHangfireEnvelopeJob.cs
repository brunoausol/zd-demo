using Hangfire;
using Microsoft.Extensions.Logging;

namespace Orders.Infrastructure.Messaging.Envelopes;

public sealed class ProcessHangfireEnvelopeJob
{
    private readonly IHangfireEnvelopeRouter _router;
    private readonly ILogger<ProcessHangfireEnvelopeJob> _logger;

    public ProcessHangfireEnvelopeJob(
        IHangfireEnvelopeRouter router,
        ILogger<ProcessHangfireEnvelopeJob> logger)
    {
        _router = router;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task ExecuteAsync(HangfireEnvelope envelope)
    {
        _logger.LogInformation(
            "Processing Hangfire envelope {EnvelopeId} ({MessageType}).",
            envelope.Id,
            envelope.MessageType);

        await _router.RouteAsync(envelope, CancellationToken.None);
    }
}
