using Microsoft.AspNetCore.Mvc;
using Orders.Api.Routing;
using Orders.Application.Contracts;
using Orders.Application.Services;

namespace Orders.Api.Controllers;

[ApiController]
[Route("orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrdersController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponse>> CreateAsync(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var created = await _orderService.CreateAsync(
            new CreateOrderRequest(
                request.CustomerName,
                request.TotalAmount),
            cancellationToken);

        var response = new OrderResponse(
            created.Id,
            created.CustomerName,
            created.TotalAmount,
            created.CreatedAtUtc);

        return CreatedAtRoute(RouteNames.Orders.GetById, new { id = response.Id }, response);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderResponse>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetAllAsync(cancellationToken);
        var response = orders
            .Select(order => new OrderResponse(
                order.Id,
                order.CustomerName,
                order.TotalAmount,
                order.CreatedAtUtc))
            .ToList();

        return Ok(response);
    }

    [HttpGet("{id:guid}", Name = RouteNames.Orders.GetById)]
    public async Task<ActionResult<OrderResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await _orderService.GetByIdAsync(id, cancellationToken);
        if (order is null)
        {
            return NotFound();
        }

        var response = new OrderResponse(
            order.Id,
            order.CustomerName,
            order.TotalAmount,
            order.CreatedAtUtc);

        return Ok(response);
    }
}
