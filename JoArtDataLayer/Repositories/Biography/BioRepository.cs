using JoArtDataLayer.Repositories.Biography.Interfaces;
using Microsoft.Extensions.Logging;

namespace JoArtDataLayer.Repositories.Biography;

public class BioRepository : IBioRepository
{
    private readonly JoArtDbContext _context;
    private readonly ILogger<BioRepository> _logger;

    public BioRepository(
        JoArtDbContext context,
        ILogger<BioRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    
}