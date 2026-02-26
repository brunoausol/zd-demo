using Orders.Abp.Application;
using Orders.Abp.Application.Contracts;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace Orders.Abp.HttpApi;

[DependsOn(
    typeof(OrdersAbpApplicationModule),
    typeof(OrdersAbpApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class OrdersAbpHttpApiModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(OrdersAbpApplicationModule).Assembly);
        });
    }
}
