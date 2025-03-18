using JoArtClassLib;

namespace JoArtDataLayer.Repositories.Interfaces;

public interface IAdminGalleryRepository
{
    public Task<Artwork> AddArtworkAsync(Artwork artwork);
    
    public Task<Artwork?> UpdateArtworkAsync(Artwork artwork);
    
    public Task<Artwork?> DeleteArtworkAsync(int artId);

    
}