using Microsoft.Extensions.Options;
using Orders.Abp.Application.BackgroundJobs;
using Orders.Abp.Application.Contracts.Integration;
using Orders.Abp.Application.Contracts.Orders;
using Orders.Abp.Application.Options;
using Orders.Abp.Domain.Entities;
using Volo.Abp.Application.Services;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;

namespace Orders.Abp.Application.Orders;

public class OrderAppService : ApplicationService, IOrderAppService
{
    private readonly IRepository<Order, Guid> _orderRepository;
    private readonly IDistributedEventBus _distributedEventBus;
    private readonly IBackgroundJobManager _backgroundJobManager;
    private readonly OrderSurveyOptions _surveyOptions;

    public OrderAppService(
        IRepository<Order, Guid> orderRepository,
        IDistributedEventBus distributedEventBus,
        IBackgroundJobManager backgroundJobManager,
        IOptions<OrderSurveyOptions> surveyOptions)
    {
        _orderRepository = orderRepository;
        _distributedEventBus = distributedEventBus;
        _backgroundJobManager = backgroundJobManager;
        _surveyOptions = surveyOptions.Value;
    }

    public async Task<OrderResponse> CreateAsync(CreateOrderRequest request)
    {
        var order = Order.Create(request.CustomerName, request.TotalAmount);
        await _orderRepository.InsertAsync(order, autoSave: true);

        var createdEvent = new OrderCreatedEto(
            order.Id,
            order.CustomerName,
            order.TotalAmount,
            order.CreationTime);

        await _distributedEventBus.PublishAsync(createdEvent);

        var delayHours = _surveyOptions.DelayHours <= 0 ? 24 : _surveyOptions.DelayHours;
        await _backgroundJobManager.EnqueueAsync(
            new SendPurchaseSurveyJobArgs
            {
                OrderId = order.Id,
                CustomerName = order.CustomerName,
                OrderCreatedAtUtc = order.CreationTime
            },
            delay: TimeSpan.FromHours(delayHours));

        return new OrderResponse(order.Id, order.CustomerName, order.TotalAmount, order.CreationTime);
    }

    public async Task<IReadOnlyList<OrderResponse>> GetAllAsync()
    {
        var orders = await _orderRepository.GetListAsync();

        return orders
            .OrderByDescending(x => x.CreationTime)
            .Select(order => new OrderResponse(order.Id, order.CustomerName, order.TotalAmount, order.CreationTime))
            .ToList();
    }

    public async Task<OrderResponse?> GetByIdAsync(Guid id)
    {
        var order = await _orderRepository.FindAsync(id);
        if (order is null)
        {
            return null;
        }

        return new OrderResponse(order.Id, order.CustomerName, order.TotalAmount, order.CreationTime);
    }
}
