using Orders.Abp.Application.Contracts;
using Orders.Abp.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Autofac;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Orders.Abp.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(OrdersAbpApplicationContractsModule),
    typeof(OrdersAbpEntityFrameworkCoreModule))]
public class OrdersAbpDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = configuration.GetConnectionString("Default");
        });

        Configure<AbpDbContextOptions>(options =>
        {
            options.UseNpgsql();
        });
    }
}
