using Orders.Abp.Domain.Shared;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Orders.Abp.Domain;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(OrdersAbpDomainSharedModule))]
public class OrdersAbpDomainModule : AbpModule
{
}
