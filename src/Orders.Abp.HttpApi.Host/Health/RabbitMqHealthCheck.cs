using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Orders.Abp.HttpApi.Host.Options;
using RabbitMQ.Client;

namespace Orders.Abp.HttpApi.Host.Health;

public class RabbitMqHealthCheck : IHealthCheck
{
    private readonly RabbitMqOptions _options;

    public RabbitMqHealthCheck(IOptions<RabbitMqOptions> options)
    {
        _options = options.Value;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _options.Host,
                Port = _options.Port,
                UserName = _options.Username,
                Password = _options.Password,
                RequestedConnectionTimeout = TimeSpan.FromSeconds(3)
            };

            using var connection = factory.CreateConnection("orders-abp-health-check");
            using var channel = connection.CreateModel();
            channel.ExchangeDeclarePassive(_options.Exchange);

            return Task.FromResult(HealthCheckResult.Healthy("RabbitMQ reachable."));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("RabbitMQ unavailable.", ex));
        }
    }
}
