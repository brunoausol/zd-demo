using Microsoft.AspNetCore.Mvc;
using Orders.Application.Contracts;
using Orders.Application.Services;

namespace Orders.Api.Controllers;

[ApiController]
[Route("api/v2/orders")]
public sealed class OrdersV2Controller : ControllerBase
{
    private readonly OrderService _orderService;

    public OrdersV2Controller(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponseV2>> CreateAsync(
        [FromBody] CreateOrderRequestV2 request,
        CancellationToken cancellationToken)
    {
        var created = await _orderService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetAllAsync), new { id = created.Id }, created);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderResponseV2>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetAllAsync(cancellationToken);
        return Ok(orders);
    }
}
