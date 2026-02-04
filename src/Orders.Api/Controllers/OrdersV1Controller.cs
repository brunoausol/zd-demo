using Microsoft.AspNetCore.Mvc;
using Orders.Application.Contracts;
using Orders.Application.Services;

namespace Orders.Api.Controllers;

[ApiController]
[Route("api/v1/orders")]
public sealed class OrdersV1Controller : ControllerBase
{
    private readonly OrderService _orderService;

    public OrdersV1Controller(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponseV1>> CreateAsync(
        [FromBody] CreateOrderRequestV1 request,
        CancellationToken cancellationToken)
    {
        var created = await _orderService.CreateAsync(
            new CreateOrderRequestV2(
                request.CustomerName,
                null,
                request.TotalAmount,
                request.Currency),
            cancellationToken);

        var response = new OrderResponseV1(
            created.Id,
            created.CustomerName,
            created.TotalAmount,
            created.Currency,
            created.CreatedAtUtc);

        return CreatedAtAction(nameof(GetAllAsync), new { id = response.Id }, response);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderResponseV1>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetAllAsync(cancellationToken);
        var response = orders
            .Select(order => new OrderResponseV1(
                order.Id,
                order.CustomerName,
                order.TotalAmount,
                order.Currency,
                order.CreatedAtUtc))
            .ToList();

        return Ok(response);
    }
}
