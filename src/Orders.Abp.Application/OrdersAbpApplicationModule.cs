using Orders.Abp.Application.Contracts;
using Orders.Abp.Domain;
using Volo.Abp.Application;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace Orders.Abp.Application;

[DependsOn(
    typeof(AbpDddApplicationModule),
    typeof(AbpBackgroundJobsAbstractionsModule),
    typeof(OrdersAbpApplicationContractsModule),
    typeof(OrdersAbpDomainModule))]
public class OrdersAbpApplicationModule : AbpModule
{
}
