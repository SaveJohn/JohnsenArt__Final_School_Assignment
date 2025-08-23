using JoArtClassLib.About;

namespace JohnsenArtAPI.Features.Biography.AdminAccess.Interfaces;

public interface IAdminBioService
{
    Task<BioBlock?> UploadBioBlockAsync(BioBlockRequest? bioBlock);

    Task<BioBlock> UpdateBioBlockAsync(UpdateBioBlockRequest bioBlock);

    Task<BioBlock> DeleteBioBlockAsync(int bioBlockId);
}