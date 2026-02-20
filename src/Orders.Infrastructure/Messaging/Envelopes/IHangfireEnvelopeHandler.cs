namespace Orders.Infrastructure.Messaging.Envelopes;

public interface IHangfireEnvelopeHandler
{
    string MessageType { get; }

    Task HandleAsync(HangfireEnvelope envelope, CancellationToken cancellationToken);
}
