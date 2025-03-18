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
        _logger.LogInformation("-------------------- \n Repository AddArtwork:");
        // Trying to save Artwork to Database
        try
        {
            _context.Artworks.Add(artwork);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database update failed while saving artwork {artworkTitle}.", artwork.ArtTitle);
            throw new Exception("Failed to save artwork due to database error. Please try again.");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation while saving artwork {artworkTitle}.", artwork.ArtTitle);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while saving artwork {artworkTitle}.", artwork.ArtTitle);
            throw;
        }
        
        return artwork;
    }
    
    // Get all Artworks
    public async Task<List<Artwork>> GetArtworksAsync()
    {
        return await _context.Artworks.Include(a => a.Images).ToListAsync();
    }
}