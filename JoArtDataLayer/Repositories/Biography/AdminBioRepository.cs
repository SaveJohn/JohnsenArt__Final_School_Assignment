using JoArtDataLayer.Repositories.Biography.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JoArtDataLayer.Repositories.Biography;

public class AdminBioRepository : IAdminBioRepository
{
    private readonly JoArtDbContext _context;
    private readonly ILogger<AdminBioRepository> _logger;

    public AdminBioRepository(
        JoArtDbContext context, 
        ILogger<AdminBioRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    
}