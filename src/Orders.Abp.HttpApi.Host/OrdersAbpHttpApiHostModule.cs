using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Orders.Abp.Application;
using Orders.Abp.Application.Options;
using Orders.Abp.EntityFrameworkCore;
using Orders.Abp.HttpApi;
using Orders.Abp.HttpApi.Host.Health;
using Orders.Abp.HttpApi.Host.Options;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BackgroundJobs.Hangfire;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EventBus.RabbitMq;
using Volo.Abp.Modularity;
using Volo.Abp.RabbitMQ;
using Volo.Abp.Swashbuckle;

namespace Orders.Abp.HttpApi.Host;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreMvcModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpEventBusRabbitMqModule),
    typeof(AbpBackgroundJobsHangfireModule),
    typeof(OrdersAbpHttpApiModule),
    typeof(OrdersAbpApplicationModule),
    typeof(OrdersAbpEntityFrameworkCoreModule))]
public class OrdersAbpHttpApiHostModule : AbpModule
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

        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = true;
        });

        Configure<OrderSurveyOptions>(configuration.GetSection(OrderSurveyOptions.SectionName));
        Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));
        Configure<HangfireDashboardOptions>(configuration.GetSection(HangfireDashboardOptions.SectionName));

        var rabbitOptions = configuration.GetSection(RabbitMqOptions.SectionName).Get<RabbitMqOptions>() ?? new RabbitMqOptions();

        Configure<AbpRabbitMqOptions>(options =>
        {
            options.Connections.Default.HostName = rabbitOptions.Host;
            options.Connections.Default.Port = rabbitOptions.Port;
            options.Connections.Default.UserName = rabbitOptions.Username;
            options.Connections.Default.Password = rabbitOptions.Password;
        });

        Configure<AbpRabbitMqEventBusOptions>(options =>
        {
            options.ClientName = "orders-abp";
            options.ExchangeName = rabbitOptions.Exchange;
            options.ConnectionName = "Default";
        });

        context.Services.AddHangfire(configuration =>
        {
            var connectionString = context.Services.GetConfiguration().GetConnectionString("Default")
                ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");

            configuration.UsePostgreSqlStorage(storage => storage.UseNpgsqlConnection(connectionString));
        });

        context.Services.AddAbpSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Orders ABP API",
                Version = "v1"
            });
        });

        context.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
            .AddDbContextCheck<OrdersAbpDbContext>("database", tags: ["ready"])
            .AddCheck<RabbitMqHealthCheck>("rabbitmq", tags: ["ready"]);
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseAbpSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Orders ABP API");
            });
        }

        var dashboardPath = app.ApplicationServices
            .GetRequiredService<IOptions<HangfireDashboardOptions>>()
            .Value
            .DashboardPath;

        app.UseHangfireDashboard(string.IsNullOrWhiteSpace(dashboardPath) ? "/hangfire" : dashboardPath);

        app.UseConfiguredEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = registration => registration.Tags.Contains("live")
            });

            endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = registration => registration.Tags.Contains("ready")
            });

            endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = registration => registration.Tags.Contains("ready")
            });
        });
    }
}
