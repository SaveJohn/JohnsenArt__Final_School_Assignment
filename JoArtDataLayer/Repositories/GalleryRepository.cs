using JoArtClassLib;
using JoArtClassLib.Art.Artwork;
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
    public async Task<IEnumerable<Artwork>> GetArtworksAsync(int page, int perPage, GallerySort sort, GalleryFilter filter)
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
            switch (filter)
            {
                case GalleryFilter.ForSale :
                    query = query.Where(a => a.ForSale == true);
                    break;
                case GalleryFilter.NotForSale :
                    query = query.Where(a => a.ForSale == false);
                    break;
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
                    query = query.OrderByDescending(a => a.Price);
                    break;
                case GallerySort.LowPrice :
                    query = query.OrderBy(a => a.Price);
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

    public async Task<Neighbors> GetGalleryNeighborsAsync(int artId)
    {
        var allIds = await _context.Artworks
            .OrderBy(a => a.Id)    
            .Select(a => a.Id)
            .ToListAsync();
        
        var idx = allIds.IndexOf(artId);
        if (idx < 0)
        {
            return new Neighbors { PreviousId = null, NextId = null };
        }
        
        return new Neighbors {
            PreviousId = idx < allIds.Count - 1 ? allIds[idx + 1] : (int?)null,
            NextId = idx > 0 ? allIds[idx - 1] : (int?)null
                
        };
    }

    public async Task<IEnumerable<string?>> GetRotationObjectKeys()
    {
        var keys = await _context.Artworks
            .Where(a => a.HomePageRotation)
            .Select(a => a.Images
                .OrderBy(img => img.Id)
                .Select(img => img.ObjectKey)
                .FirstOrDefault())
            .Where(key => key != null)
            .ToListAsync();
        
        return keys;
    }
}