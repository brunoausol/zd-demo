using Microsoft.Extensions.DependencyInjection;
using Orders.Application.Services;

namespace Orders.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<OrderService>();
        return services;
    }
}
