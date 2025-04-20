using JoArtDataLayer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JoArtDataLayer.Repositories;

public class AdminDetailRepository : IAdminDetailRepository
{
    private readonly JoArtDbContext _dbContext;
    private readonly ILogger<AdminDetailRepository> _logger;

    public AdminDetailRepository(
        JoArtDbContext dbContext,
        ILogger<AdminDetailRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<string> GetAdminEmail()
    {
        _logger.LogInformation("-------------------- \n Repository : Get Admin Email:");
        try
        {
            var admin = await _dbContext.Admins.OrderByDescending(a => a.Email).FirstOrDefaultAsync();

            if (admin is null)
            {
                _logger.LogInformation("No admin found");
                return string.Empty;
            }
            if (string.IsNullOrWhiteSpace(admin.Email))
            {
                _logger.LogInformation("No admin email found");
                return string.Empty;
            }
            
            _logger.LogInformation($"Got email: {admin.Email}");
            return admin.Email;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database query failed due to an invalid operation.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve Admin from the database.");
            throw;
        }
    }
}