using JoArtClassLib.About;

namespace JoArtDataLayer.Repositories.Biography.Interfaces;

public interface IAdminBioRepository
{
    Task<BioBlock?> UploadBioBlockAsync(BioBlock? bioBlock);

    Task<BioBlock> UpdateBioBlockAsync(BioBlock bioBlock);

    Task<BioBlock> DeleteBioBlockAsync(int bioBlockId);
}