using Orders.Application.Contracts;

namespace Orders.Application.Abstractions;

public interface IOrderPublisher
{
    Task PublishAsync(OrderCreatedEvent message, CancellationToken cancellationToken);
}
