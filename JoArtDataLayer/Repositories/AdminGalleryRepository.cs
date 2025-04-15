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

    // ADD Artwork
    public async Task<Artwork> AddArtworkAsync(Artwork artwork)
    {
        _logger.LogInformation("-------------------- \n Repository : AddArtwork:");
        // Trying to save Artwork to Database
        try
        {
            _context.Artworks.Add(artwork);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database update failed while saving artwork {artworkTitle}.", artwork.Title);
            throw new Exception("Failed to save artwork due to database error. Please try again.");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation while saving artwork {artworkTitle}.", artwork.Title);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while saving artwork {artworkTitle}.", artwork.Title);
            throw;
        }

        return artwork;
    }

    // UPDATE Artwork
    public async Task<Artwork?> UpdateArtworkAsync(Artwork artwork)
    {
        _logger.LogInformation("-------------------- \n Repository : UpdateArtwork:");
        // Checking if Artwork exists in database
        var existingArtwork = await _context.Artworks
            .Include(a => a.Images) 
            .FirstOrDefaultAsync(a => a.Id == artwork.Id);
        if (existingArtwork == null)
        {
            _logger.LogWarning("Attempted to update non-existing artwork with ID {artworkId}", artwork.Id);
            return null;
        }

        // Updating Artwork in database
        try
        {
            _context.Entry(existingArtwork).CurrentValues.SetValues(artwork);
            _context.Entry(existingArtwork).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully updated artwork {artworkId}", artwork.Id);
            return existingArtwork;

        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database update failed for artwork {artworkId}", artwork.Id);
            throw new Exception("Failed to update artwork due to a database error.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while updating artwork {artworkId}", artwork.Id);
            throw;
        }

    }

    // DELETE Artwork
    public async Task<Artwork?> DeleteArtworkAsync(int artId)
    {
        _logger.LogInformation("-------------------- \n Repository : DeleteArtwork:");

        var existingArtwork = await _context.Artworks.FindAsync(artId);
        if (existingArtwork == null)
        {
            _logger.LogWarning("Attempted to delete non-existing artwork with ID {artId}", artId);
            return null;
        }

        try
        {
            await _context.Artworks
                .Where(a => a.Id == artId)
                .ExecuteDeleteAsync();

            _logger.LogInformation("Successfully deleted artwork {artId}", artId);
            return existingArtwork;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while deleting artwork {artId}", artId);
            throw new Exception("Failed to delete artwork due to a database error.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while deleting artwork {artId}", artId);
            throw;
        }


    }
    
    

    // GET Image Object Key
    public async Task<string?> GetObjectKeyByImageIdAsync(int? imageId)
    {
        _logger.LogInformation($"-------------------- \n Repository : GetObjectKeyByImageId: {imageId}:");
        try
        {
            return await _context.ArtworkImages
                .Where(i => i.Id == imageId)
                .Select(i => i.ObjectKey)
                .FirstOrDefaultAsync();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Database query failed due to an invalid operation.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve object key from the database.");
            throw;
        }

    }

}