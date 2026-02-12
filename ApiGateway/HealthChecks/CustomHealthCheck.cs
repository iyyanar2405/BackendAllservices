using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ApiGateway.HealthChecks;

public class CustomHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        // Implement custom health check logic
        return Task.FromResult(HealthCheckResult.Healthy("API Gateway is healthy"));
    }
}
