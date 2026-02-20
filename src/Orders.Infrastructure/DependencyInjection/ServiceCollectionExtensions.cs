using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        services.Configure<HangfireOptions>(configuration.GetSection(HangfireOptions.SectionName));

        var connectionString = configuration.GetConnectionString("Orders");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Database connection string 'Orders' is not configured.");

        services.AddDbContext<OrdersDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql => npgsql.MigrationsAssembly(typeof(OrdersDbContext).Assembly.FullName)));

        var hangfireOptions = configuration.GetSection(HangfireOptions.SectionName).Get<HangfireOptions>() ?? new HangfireOptions();

        services.AddHangfire(hangfireConfiguration => hangfireConfiguration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(
                options => options.UseNpgsqlConnection(connectionString),
                new PostgreSqlStorageOptions
                {
                    QueuePollInterval = TimeSpan.FromSeconds(3),
                    InvisibilityTimeout = TimeSpan.FromMinutes(1),
                    PrepareSchemaIfNecessary = true
                }));

        services.AddHangfireServer(options =>
        {
            options.WorkerCount = hangfireOptions.WorkerCount <= 0
                ? Environment.ProcessorCount
                : hangfireOptions.WorkerCount;
            options.ShutdownTimeout = hangfireOptions.ShutdownTimeoutSeconds <= 0
                ? TimeSpan.FromSeconds(30)
                : TimeSpan.FromSeconds(hangfireOptions.ShutdownTimeoutSeconds);
        });

        var shutdownTimeoutSeconds = Math.Max(
            hangfireOptions.ShutdownTimeoutSeconds <= 0 ? 30 : hangfireOptions.ShutdownTimeoutSeconds,
            configuration.GetSection(RabbitMqOptions.SectionName).Get<RabbitMqOptions>()?.ShutdownDrainTimeoutSeconds ?? 30);

        services.Configure<HostOptions>(options =>
        {
            options.ShutdownTimeout = TimeSpan.FromSeconds(shutdownTimeoutSeconds);
        });

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddSingleton<IOrderPublisher, RabbitMqOrderPublisher>();
        services.AddScoped<IOrderSurveyScheduler, HangfireOrderSurveyScheduler>();
        services.AddSingleton<IOrderConfirmationEmailSender, OrderConfirmationEmailSender>();
        services.AddSingleton<IPurchaseSurveySender, PurchaseSurveySender>();
        services.AddTransient<SendPurchaseSurveyJob>();
        services.AddHostedService<RabbitMqOrderCreatedConsumer>();

        return services;
    }
}
