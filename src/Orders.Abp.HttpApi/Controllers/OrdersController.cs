using Microsoft.AspNetCore.Mvc;
using Orders.Abp.Application.Contracts.Orders;
using Volo.Abp.AspNetCore.Mvc;

namespace Orders.Abp.HttpApi.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : AbpController
{
    private readonly IOrderAppService _orderAppService;

    public OrdersController(IOrderAppService orderAppService)
    {
        _orderAppService = orderAppService;
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponse>> CreateAsync([FromBody] CreateOrderRequest request)
    {
        var created = await _orderAppService.CreateAsync(request);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = created.Id }, created);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderResponse>>> GetAllAsync()
    {
        var orders = await _orderAppService.GetAllAsync();
        return Ok(orders);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderResponse>> GetByIdAsync(Guid id)
    {
        var order = await _orderAppService.GetByIdAsync(id);
        if (order is null)
        {
            return NotFound();
        }

        return Ok(order);
    }
}
