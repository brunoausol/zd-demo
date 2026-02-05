using Orders.Application.Abstractions;
using Orders.Application.Contracts;
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

    public async Task<OrderResponse> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var order = Order.Create(request.FirstName, request.LastName, request.TotalAmount);

        await _orders.AddAsync(order, cancellationToken);
        await _orders.SaveChangesAsync(cancellationToken);

        var message = new OrderCreatedEvent(
            order.Id,
            order.TotalAmount,
            order.CreatedAtUtc);

        await _publisher.PublishAsync(message, cancellationToken);

        return new OrderResponse(
            order.Id,
            order.FirstName,
            order.LastName,
            order.TotalAmount,
            order.CreatedAtUtc);
    }

    public async Task<IReadOnlyList<OrderResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var orders = await _orders.GetAllAsync(cancellationToken);
        return orders
            .Select(order => new OrderResponse(
                order.Id,
                order.FirstName,
                order.LastName,
                order.TotalAmount,
                order.CreatedAtUtc))
            .ToList();
    }

    public async Task<OrderResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await _orders.GetByIdAsync(id, cancellationToken);
        if (order is null)
        {
            return null;
        }
        return new OrderResponse(
            order.Id,
            order.FirstName,
            order.LastName,
            order.TotalAmount,
            order.CreatedAtUtc);
    }
}
