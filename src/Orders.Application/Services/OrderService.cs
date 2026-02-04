using Orders.Application.Abstractions;
using Orders.Application.Contracts;
using Orders.Application.Events;
using Orders.Domain.Entities;

namespace Orders.Application.Services;

public sealed class OrderService
{
    private readonly IOrderRepository _orders;
    private readonly IOrderPublisher _publisher;

    public OrderService(IOrderRepository orders, IOrderPublisher publisher)
    {
        _orders = orders;
        _publisher = publisher;
    }

    public async Task<OrderResponseV2> CreateAsync(CreateOrderRequestV2 request, CancellationToken cancellationToken)
    {
        var order = Order.Create(request.CustomerName, request.CustomerEmail, request.TotalAmount, request.Currency ?? "EUR");

        await _orders.AddAsync(order, cancellationToken);
        await _orders.SaveChangesAsync(cancellationToken);

        var message = new OrderCreatedEvent(
            order.Id,
            order.CustomerName,
            order.CustomerEmail,
            order.TotalAmount,
            order.Currency,
            order.CreatedAtUtc);

        await _publisher.PublishAsync(message, cancellationToken);

        return new OrderResponseV2(
            order.Id,
            order.CustomerName,
            order.CustomerEmail,
            order.TotalAmount,
            order.Currency,
            order.CreatedAtUtc);
    }

    public async Task<IReadOnlyList<OrderResponseV2>> GetAllAsync(CancellationToken cancellationToken)
    {
        var orders = await _orders.GetAllAsync(cancellationToken);
        return orders
            .Select(order => new OrderResponseV2(
                order.Id,
                order.CustomerName,
                order.CustomerEmail,
                order.TotalAmount,
                order.Currency,
                order.CreatedAtUtc))
            .ToList();
    }
}
