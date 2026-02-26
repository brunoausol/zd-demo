using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Orders.Abp.EntityFrameworkCore;

public class OrdersAbpDbContextFactory : IDesignTimeDbContextFactory<OrdersAbpDbContext>
{
    public OrdersAbpDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrdersAbpDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=orders;Username=postgres;Password=postgres");
        return new OrdersAbpDbContext(optionsBuilder.Options);
    }
}
