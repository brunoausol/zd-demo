using Microsoft.Extensions.Logging;
using Orders.Abp.Application.Contracts.Integration;
using Volo.Abp.DependencyInjection;

namespace Orders.Abp.Application.Messaging;

public class OrderConfirmationEmailSender : IOrderConfirmationEmailSender, ITransientDependency
{
    private readonly ILogger<OrderConfirmationEmailSender> _logger;

    public OrderConfirmationEmailSender(ILogger<OrderConfirmationEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(OrderCreatedEto message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[EMAIL] Order confirmation sent for OrderId={OrderId}, Customer={CustomerName}, Total={TotalAmount}.",
            message.Id,
            message.CustomerName,
            message.TotalAmount);

        return Task.CompletedTask;
    }
}
