using Microsoft.EntityFrameworkCore;
using Orders.Domain.Entities;
using Orders.Infrastructure.Data.Configurations;

namespace Orders.Infrastructure.Data;

public sealed class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
    }
}
