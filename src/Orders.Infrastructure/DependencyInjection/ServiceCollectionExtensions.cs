using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orders.Application.Abstractions;
using Orders.Infrastructure.Data;
using Orders.Infrastructure.Messaging;
using Orders.Infrastructure.Options;
using Orders.Infrastructure.Repositories;

namespace Orders.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));

        var connectionString = configuration.GetConnectionString("Orders");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Database connection string 'Orders' is not configured.");

        services.AddDbContext<OrdersDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql => npgsql.MigrationsAssembly(typeof(OrdersDbContext).Assembly.FullName)));

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddSingleton<IOrderPublisher, RabbitMqPublisher>();

        return services;
    }
}
