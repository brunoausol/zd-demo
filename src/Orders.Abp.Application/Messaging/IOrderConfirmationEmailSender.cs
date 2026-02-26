using Orders.Abp.Application.Contracts.Integration;

namespace Orders.Abp.Application.Messaging;

public interface IOrderConfirmationEmailSender
{
    Task SendAsync(OrderCreatedEto message, CancellationToken cancellationToken = default);
}
