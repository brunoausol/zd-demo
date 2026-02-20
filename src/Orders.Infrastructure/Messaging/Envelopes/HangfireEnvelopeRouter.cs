using Microsoft.Extensions.Logging;

namespace Orders.Infrastructure.Messaging.Envelopes;

public sealed class HangfireEnvelopeRouter : IHangfireEnvelopeRouter
{
    private readonly ILogger<HangfireEnvelopeRouter> _logger;
    private readonly IReadOnlyDictionary<string, IHangfireEnvelopeHandler> _handlers;

    public HangfireEnvelopeRouter(
        IEnumerable<IHangfireEnvelopeHandler> handlers,
        ILogger<HangfireEnvelopeRouter> logger)
    {
        _logger = logger;
        _handlers = handlers
            .GroupBy(handler => handler.MessageType, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group =>
            {
                if (group.Count() > 1)
                {
                    throw new InvalidOperationException($"Multiple Hangfire handlers registered for '{group.Key}'.");
                }

                return group.Single();
            }, StringComparer.OrdinalIgnoreCase);
    }

    public Task RouteAsync(HangfireEnvelope envelope, CancellationToken cancellationToken)
    {
        if (!_handlers.TryGetValue(envelope.MessageType, out var handler))
        {
            throw new InvalidOperationException($"No Hangfire handler registered for '{envelope.MessageType}'.");
        }

        _logger.LogInformation(
            "Routing Hangfire envelope {EnvelopeId} with message type {MessageType}.",
            envelope.Id,
            envelope.MessageType);

        return handler.HandleAsync(envelope, cancellationToken);
    }
}
