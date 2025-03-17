using JoArtClassLib;
using JoArtDataLayer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JoArtDataLayer.Repositories;

public class AdminGalleryRepository : IAdminGalleryRepository
{
    private readonly JoArtDbContext _context;
    private readonly ILogger<AdminGalleryRepository> _logger;

    public AdminGalleryRepository(
        JoArtDbContext context,
        ILogger<AdminGalleryRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    // Upload a Artwork
    public async Task<Artwork> AddArtworkAsync(Artwork artwork)
    {
        foreach (var image in artwork.Images)
        {
            _logger.LogInformation("ObjectKey {objectKey}", image.ObjectKey);
        }
        // Trying to save Artwork to Database
        try
        {
            _context.Artworks.Add(artwork);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw; // Could not save Artwork to Database
        }
        // Return artwork
        return artwork;
    }
    
    // Get all Artworks
    public async Task<List<Artwork>> GetArtworksAsync()
    {
        return await _context.Artworks.Include(a => a.Images).ToListAsync();
    }
}