using JoArtClassLib;
using JoArtDataLayer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JoArtDataLayer.Repositories;

public class AdminUserRepository : IAdminUserRepository
{
    private readonly JoArtDbContext _dbContext;
    private readonly ILogger<AdminUserRepository> _logger;

    public AdminUserRepository(
        JoArtDbContext dbContext,
        ILogger<AdminUserRepository> logger)
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
                _logger.LogError("No admin found");
                throw new Exception("No admin found");
            }
            if (string.IsNullOrWhiteSpace(admin.Email))
            {
                _logger.LogError("No admin email found");
                throw new Exception("No admin found");
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

    public async Task<Admin> GetAdmin(string email)
    {
        _logger.LogInformation("-------------------- \n Repository : Get Hashed Password:");
        try
        {
            var admin = await _dbContext.Admins
                .Where(a => a.Email == email)
                .FirstOrDefaultAsync();
            
            _logger.LogInformation($"Got admin: {admin.Name}");
            return admin;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database query failed due to an invalid operation.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to admin from the database.");
            throw;
        }
    }
}