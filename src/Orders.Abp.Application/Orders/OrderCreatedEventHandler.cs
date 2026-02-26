using Orders.Abp.Application.Contracts.Integration;
using Orders.Abp.Application.Messaging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Orders.Abp.Application.Orders;

public class OrderCreatedEventHandler : IDistributedEventHandler<OrderCreatedEto>, ITransientDependency
{
    private readonly IOrderConfirmationEmailSender _emailSender;

    public OrderCreatedEventHandler(IOrderConfirmationEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public async Task HandleEventAsync(OrderCreatedEto eventData)
    {
        await _emailSender.SendAsync(eventData);
    }
}
