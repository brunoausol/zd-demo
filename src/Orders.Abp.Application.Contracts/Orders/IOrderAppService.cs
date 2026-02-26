using Volo.Abp.Application.Services;

namespace Orders.Abp.Application.Contracts.Orders;

public interface IOrderAppService : IApplicationService
{
    Task<OrderResponse> CreateAsync(CreateOrderRequest request);
    Task<IReadOnlyList<OrderResponse>> GetAllAsync();
    Task<OrderResponse?> GetByIdAsync(Guid id);
}
