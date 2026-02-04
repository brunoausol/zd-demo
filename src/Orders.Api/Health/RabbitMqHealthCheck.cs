using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Orders.Infrastructure.Options;
using RabbitMQ.Client;

namespace Orders.Api.Health;

public sealed class RabbitMqHealthCheck : IHealthCheck
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
                Password = _options.Password
            };

            using var connection = factory.CreateConnection();
            return Task.FromResult(connection.IsOpen
                ? HealthCheckResult.Healthy("RabbitMQ reachable.")
                : HealthCheckResult.Unhealthy("RabbitMQ connection closed."));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("RabbitMQ check failed.", ex));
        }
    }
}
