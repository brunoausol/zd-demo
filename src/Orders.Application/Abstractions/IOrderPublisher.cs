using Orders.Application.Events;

namespace Orders.Application.Abstractions;

public interface IOrderPublisher
{
    Task PublishAsync(OrderCreatedEvent message, CancellationToken cancellationToken);
}
