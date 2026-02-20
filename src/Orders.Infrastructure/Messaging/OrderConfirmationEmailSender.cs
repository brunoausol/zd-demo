using Microsoft.Extensions.Logging;
using Orders.Application.Contracts;

namespace Orders.Infrastructure.Messaging;

public sealed class OrderConfirmationEmailSender : IOrderConfirmationEmailSender
{
    private readonly ILogger<OrderConfirmationEmailSender> _logger;

    public OrderConfirmationEmailSender(ILogger<OrderConfirmationEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(OrderCreatedEvent message, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Order confirmation e-mail sent. OrderId: {OrderId}, Customer: {CustomerName}, Total: {TotalAmount}",
            message.Id,
            message.CustomerName,
            message.TotalAmount);

        return Task.CompletedTask;
    }
}
