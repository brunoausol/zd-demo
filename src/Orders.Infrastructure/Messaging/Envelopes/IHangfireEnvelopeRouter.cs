namespace Orders.Infrastructure.Messaging.Envelopes;

public interface IHangfireEnvelopeRouter
{
    Task RouteAsync(HangfireEnvelope envelope, CancellationToken cancellationToken);
}
