using System.Text.Json;

namespace Orders.Infrastructure.Messaging.Envelopes;

public sealed record HangfireEnvelope(
    Guid Id,
    string MessageType,
    DateTime CreatedAtUtc,
    string Payload)
{
    public static HangfireEnvelope Create<TPayload>(string messageType, TPayload payload)
    {
        return new HangfireEnvelope(
            Id: Guid.NewGuid(),
            MessageType: messageType,
            CreatedAtUtc: DateTime.UtcNow,
            Payload: JsonSerializer.Serialize(payload));
    }

    public TPayload DeserializePayload<TPayload>()
    {
        var payload = JsonSerializer.Deserialize<TPayload>(Payload);
        if (payload is null)
        {
            throw new InvalidOperationException(
                $"Envelope {Id} payload cannot be deserialized to {typeof(TPayload).Name}.");
        }

        return payload;
    }
}
