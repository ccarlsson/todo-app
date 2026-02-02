using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TodoApp.Api.Health;

public sealed class MongoHealthCheck : IHealthCheck
{
    private readonly IConfiguration _configuration;

    public MongoHealthCheck(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var connectionString = _configuration.GetSection("MongoDb")["ConnectionString"];
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return HealthCheckResult.Healthy("MongoDb not configured.");
        }

        try
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("admin");
            var command = new BsonDocument("ping", 1);
            await database.RunCommandAsync<BsonDocument>(command, cancellationToken: cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("MongoDb connection failed.", ex);
        }
    }
}
