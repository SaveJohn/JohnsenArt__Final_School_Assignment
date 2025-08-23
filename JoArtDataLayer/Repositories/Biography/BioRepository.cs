using JoArtClassLib.About;
using JoArtClassLib.Enums;
using JoArtDataLayer.Repositories.Biography.Interfaces;
using Microsoft.EntityFrameworkCore;
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

    public async Task<List<BioBlock>> GetBioBlocksAsync()
    {
        _logger.LogInformation($"-------------------- \n Repository : GetBioBlocks:");
        
        try
        {
            // Query
            var query = _context.BioBlocks
                .Include(a => a.Images)
                .AsQueryable();
            
            _logger.LogInformation("Successfully retrieved Bio Blocks from the database.");

            return await query.ToListAsync();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database query failed due to an invalid operation.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve Bio Blocks from the database.");
            throw;
        }
    }

    public async Task<BioBlock?> GetBioBlockByIdAsync(int bioBlockId)
    {
        _logger.LogInformation($"-------------------- \n Repository : GetBioBlockById:");
        
        try
        {
            _logger.LogInformation($"Retrieving Bio Block by ID: {bioBlockId}");
            return await _context.BioBlocks
                .Include(a => a.Images)
                .Where(a => a.Id == bioBlockId)
                .FirstOrDefaultAsync();;
            
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database query failed due to an invalid operation.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve Bio Blocks from the database.");
            throw;
        }
    }
}