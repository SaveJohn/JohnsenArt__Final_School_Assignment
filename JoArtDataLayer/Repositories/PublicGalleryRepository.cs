using JoArtClassLib;
using JoArtClassLib.Art;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JoArtDataLayer.Repositories;

public class PublicGalleryRepository
{
    private readonly JoArtDbContext _dbContext;
    private readonly ILogger<PublicGalleryRepository> _logger;

    public PublicGalleryRepository(JoArtDbContext dbContext, ILogger<PublicGalleryRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    // Get all artworks
    public async Task<List<Artwork>> GetAllArtworksAsync()
    {
        return await _dbContext.Artworks
            .Include(a => a.Images) 
            .ToListAsync();
    }
    
    // Get artwork by ID
    public async Task<List<Artwork>> GetArtworkByIdAsync()
    {
        throw new NotImplementedException();
    }
    
    // Filter
}