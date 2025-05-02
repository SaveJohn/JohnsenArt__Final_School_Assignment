using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace JohnsenArtAPI.Features.Health;

public class APIHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return await Task.FromResult(HealthCheckResult.Healthy("Api is running."));
    }
}