using Orders.Abp.HttpApi.Host;
using Serilog;
using Volo.Abp;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseAutofac();
    builder.Host.UseSerilog();

    await builder.AddApplicationAsync<OrdersAbpHttpApiHostModule>();

    var app = builder.Build();
    await app.InitializeApplicationAsync();
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Orders ABP host terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}
