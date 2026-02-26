using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orders.Abp.DbMigrator;
using Serilog;
using Volo.Abp;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    using var app = Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureServices((_, services) =>
        {
            services.AddApplication<OrdersAbpDbMigratorModule>();
        })
        .Build();
    await app.InitializeApplicationAsync();

    await app.Services
        .GetRequiredService<OrdersAbpDbMigrationService>()
        .MigrateAsync();

    await app.ShutdownAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Database migration failed.");
}
finally
{
    Log.CloseAndFlush();
}
