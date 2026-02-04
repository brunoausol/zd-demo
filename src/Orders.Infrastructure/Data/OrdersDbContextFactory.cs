using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Orders.Infrastructure.Data;

public sealed class OrdersDbContextFactory : IDesignTimeDbContextFactory<OrdersDbContext>
{
    public OrdersDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("Orders") ??
                               "Host=localhost;Port=5432;Database=orders;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<OrdersDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new OrdersDbContext(optionsBuilder.Options);
    }
}
