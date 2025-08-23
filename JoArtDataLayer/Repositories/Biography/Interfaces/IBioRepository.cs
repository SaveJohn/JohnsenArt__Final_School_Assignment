using JoArtClassLib.About;

namespace JoArtDataLayer.Repositories.Biography.Interfaces;

public interface IBioRepository
{
    Task<List<BioBlock>> GetBioBlocksAsync();
    Task<BioBlock?> GetBioBlockByIdAsync(int bioBlockId);
}