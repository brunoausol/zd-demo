using Orders.Abp.Domain.Shared;
using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Orders.Abp.Application.Contracts;

[DependsOn(
    typeof(AbpDddApplicationContractsModule),
    typeof(OrdersAbpDomainSharedModule))]
public class OrdersAbpApplicationContractsModule : AbpModule
{
}
