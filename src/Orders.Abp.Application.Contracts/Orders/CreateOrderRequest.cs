using System.ComponentModel.DataAnnotations;

namespace Orders.Abp.Application.Contracts.Orders;

public record CreateOrderRequest(
    [property: Required, MinLength(2)] string CustomerName,
    [property: Range(0.01, 999999999)] decimal TotalAmount);
