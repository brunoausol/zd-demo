using Microsoft.EntityFrameworkCore;
using Orders.Application.Abstractions;
using Orders.Domain.Entities;
using Orders.Infrastructure.Data;

namespace Orders.Infrastructure.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly OrdersDbContext _dbContext;

    public OrderRepository(OrdersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(Order order, CancellationToken cancellationToken)
        => _dbContext.Orders.AddAsync(order, cancellationToken).AsTask();

    public async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken)
        => await _dbContext.Orders.AsNoTracking().OrderByDescending(order => order.CreatedAtUtc).ToListAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
