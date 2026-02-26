using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;

namespace Orders.Abp.DbMigrator;

public class OrdersAbpDbMigrationService : ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OrdersAbpDbMigrationService> _logger;

    public OrdersAbpDbMigrationService(
        IServiceProvider serviceProvider,
        ILogger<OrdersAbpDbMigrationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task MigrateAsync()
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
        await using var uow = uowManager.Begin(requiresNew: true, isTransactional: false);

        var dbContext = scope.ServiceProvider.GetRequiredService<OrdersAbpDbContext>();
        _logger.LogInformation("Applying EF Core migrations...");
        await dbContext.Database.MigrateAsync();

        await uow.CompleteAsync();
        _logger.LogInformation("Database migration completed.");
    }
}
