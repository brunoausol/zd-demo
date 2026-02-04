using Microsoft.Extensions.Diagnostics.HealthChecks;
using Orders.Infrastructure.Data;

namespace Orders.Api.Health;

public sealed class DatabaseHealthCheck : IHealthCheck
{
    private readonly OrdersDbContext _dbContext;

    public DatabaseHealthCheck(OrdersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);
            return canConnect
                ? HealthCheckResult.Healthy("Database reachable.")
                : HealthCheckResult.Unhealthy("Database unreachable.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database check failed.", ex);
        }
    }
}
