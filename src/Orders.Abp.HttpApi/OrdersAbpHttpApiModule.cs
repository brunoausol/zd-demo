using Orders.Abp.Application.Contracts;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace Orders.Abp.HttpApi;

[DependsOn(
    typeof(OrdersAbpApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class OrdersAbpHttpApiModule : AbpModule
{
}
