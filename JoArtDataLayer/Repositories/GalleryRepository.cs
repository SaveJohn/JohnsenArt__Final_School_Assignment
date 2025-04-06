using JoArtClassLib;
using JoArtClassLib.Enums;
using JoArtDataLayer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JoArtDataLayer.Repositories;

public class GalleryRepository : IGalleryRepository
{
    private readonly JoArtDbContext _context;
    private readonly ILogger<GalleryRepository> _logger;

    public GalleryRepository(
        JoArtDbContext context, 
        ILogger<GalleryRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    // GET Artworks
    public async Task<IEnumerable<Artwork>> GetArtworksAsync(int page, int perPage, GallerySort sort, bool? forSale)
    {
        _logger.LogInformation($"-------------------- \n Repository : GetArtworks:");
        
        // Paging
        int skip = (page - 1) * perPage;
        try
        {
            // Query
            var query = _context.Artworks
                .Include(a => a.Images)
                .AsQueryable();

            // Filter
            if (forSale.HasValue)
            {
                query = query.Where(a => a.ForSale == forSale.Value);
            }

            // Apply sorting
            switch (sort)
            {
                case GallerySort.Newest :
                    query = query.OrderByDescending(a => a.Id);
                    break;
                case GallerySort.Oldest :
                    query = query.OrderBy(a => a.Id);
                    break;
                case GallerySort.HighPrice :
                    query = query.OrderBy(a => a.Price);
                    break;
                case GallerySort.LowPrice :
                    query = query.OrderByDescending(a => a.Price);
                    break;
                    
            }
            
            _logger.LogInformation("Successfully retrieved Artworks from the database.");

            return await query.Skip(skip).Take(perPage).ToListAsync();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid paging parameters provided.");
            throw;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database query failed due to an invalid operation.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve Artworks from the database.");
            throw;
        }
        
    }
    
    // GET Artwork by Id
    public async Task<Artwork?> GetArtworkByIdAsync(int artId)
    {
        _logger.LogInformation($"-------------------- \n Repository : GetArtworkById: {artId}:");

        try
        {
            _logger.LogInformation($"Retrieving Artwork by ID: {artId}");
            return await _context.Artworks
                .Include(a => a.Images)
                .Where(a => a.Id == artId)
                .FirstOrDefaultAsync();
            
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database query failed due to an invalid operation.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve Artworks from the database.");
            throw;
        }
        
    }
    
    
}