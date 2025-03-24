using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace JoArtDataLayer.Health;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly JoArtDbContext _dbContext;
    private readonly ILogger<DatabaseHealthCheck> _logger;

    public DatabaseHealthCheck(
        JoArtDbContext dbContext,
        ILogger<DatabaseHealthCheck> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = new CancellationToken())
    {
        Dictionary<string, object> results = await CheckDbConnectionAsync(cancellationToken);
        
        if (results.FirstOrDefault().Key != "ConnectionError")
        {
            // Merging table results with db connection results
            Dictionary<string, object> tableResults = await CheckTablesAccessibilityAsync(cancellationToken);
            foreach (var kv in tableResults)
            {
                results[kv.Key] = kv.Value;
            }
            _logger.LogInformation("DB Connection established:");
            foreach (var kv in results)
            {
                _logger.LogInformation($"{kv.Key}: {kv.Value}");
            }
            // Return healthy status with merged results
            return HealthCheckResult.Healthy(
                "Database is connected", 
                results);
        }
        _logger.LogError("DB Connection Error");
        foreach (var kv in results)
        {
            _logger.LogError($"{kv.Key}: {kv.Value}");
        }
        // Return unhealthy status if connection failed
        return HealthCheckResult.Unhealthy(
            "Database is disconnected",  
            new Exception("Connection failed"),
            results);
    }
    
    private async Task<Dictionary<string, object>> CheckDbConnectionAsync(CancellationToken cancellationToken)
    {
        Dictionary<string, object> results = new ();
        
        try
        {
            // Check if db can connect
            if (await _dbContext.Database.CanConnectAsync(cancellationToken))
            {
                // Adding information to results
                results.Add("DatabaseName", _dbContext.Database.GetDbConnection().Database);
                results.Add("Server", _dbContext.Database.GetDbConnection().DataSource);
            
            }
        }
        catch (Exception ex)
        {
            // Logging and adding connection error to results
            _logger.LogError($"Database connection failed. {ex.Message}" );
            results.Add("ConnectionError", ex.Message);
        }
        return results;
    }
    
    private async Task<Dictionary<string, object>> CheckTablesAccessibilityAsync(CancellationToken cancellationToken)
    {
        Dictionary<string, object> results = new();
    
        // Getting db properties
        var dbProperties = _dbContext.GetType()
            .GetProperties()
            .Where(p => p.PropertyType.IsGenericType &&
                        p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

        // Checking each property / table 
        foreach (var property in dbProperties)
        {
            var tableName = property.Name;
            try
            {
                // Getting the DbSet dynamically
                var dbSet = property.GetValue(_dbContext);

                // Cast DbSet to IQueryable and execute AnyAsync
                var query = (IQueryable<object>)dbSet;

                // Use LINQ's AnyAsync to check if there is data in the table
                var exists = await query.AnyAsync(cancellationToken);

                // Add result to the dictionary
                results.Add(tableName, exists ? "accessible" : "empty");
            }
            catch (Exception ex)
            {
                // Handle errors and mark the table as inaccessible
                results.Add(tableName, $"Table inaccessible: {ex.Message}");
            }
        }
        return results;
        
    }
}