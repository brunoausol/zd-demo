using Orders.Application.Contracts;

namespace Orders.Infrastructure.Messaging;

public interface IOrderConfirmationEmailSender
{
    Task SendAsync(OrderCreatedEvent message, CancellationToken cancellationToken);
}
