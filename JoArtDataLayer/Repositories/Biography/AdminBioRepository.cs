using JoArtClassLib.About;
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
    
    // Upload BioBlock
    public async Task<BioBlock?> UploadBioBlockAsync(BioBlock? bioBlock)
    {
        _logger.LogInformation("-------------------- \n Repository : UploadBioBlock:");
        // Trying to save Artwork to Database
        try
        {
            _context.BioBlocks.Add(bioBlock);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database update failed while saving bio block.");
            throw new Exception("Failed to save bio block due to database error. Please try again.");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation while saving bio block.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while saving bio block.");
            throw;
        }

        return bioBlock;
    }
    
    // Update BioBlock
    public async Task<BioBlock> UpdateBioBlockAsync(BioBlock bioBlock)
    {
        _logger.LogInformation("-------------------- \n Repository : UpdateBioBlock:");
        // Checking if BioBlock exists in database
        var existingBioBlock = await _context.BioBlocks
            .Include(a => a.Images) 
            .FirstOrDefaultAsync(a => a.Id == bioBlock.Id);
        if (existingBioBlock == null)
        {
            _logger.LogWarning("Attempted to update non-existing artwork with ID {bioBlockId}", bioBlock.Id);
            return null;
        }

        // Updating BioBlock in database
        try
        {
            _context.Entry(existingBioBlock).CurrentValues.SetValues(bioBlock);
            _context.Entry(existingBioBlock).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully updated bio block {bioBlockId}", bioBlock.Id);
            return existingBioBlock;

        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database update failed for bio block {bioBlockId}", bioBlock.Id);
            throw new Exception("Failed to update bio block due to a database error.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while updating artwork {bioBlockId}", bioBlock.Id);
            throw;
        }
    }
    
    // Delete BioBlock
    public async Task<BioBlock> DeleteBioBlockAsync(int bioBlockId)
    {
        _logger.LogInformation("-------------------- \n Repository : DeleteBioBlock:");

        var existingBioBlock = await _context.BioBlocks.FindAsync(bioBlockId);
        if (existingBioBlock == null)
        {
            _logger.LogWarning("Attempted to delete non-existing Bio Block with ID {bioBlockId}", bioBlockId);
            return null;
        }

        try
        {
            await _context.BioBlocks
                .Where(a => a.Id == bioBlockId)
                .ExecuteDeleteAsync();

            _logger.LogInformation("Successfully deleted Bio Block {bioBlockId}", bioBlockId);
            return existingBioBlock;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while deleting Bio Block {bioBlockId}", bioBlockId);
            throw new Exception("Failed to delete Bio Block due to a database error.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while deleting Bio Block {bioBlockId}", bioBlockId);
            throw;
        }
        
    }
}