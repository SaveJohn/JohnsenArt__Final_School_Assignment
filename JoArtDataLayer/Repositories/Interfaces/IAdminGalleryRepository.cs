using JoArtClassLib;

namespace JoArtDataLayer.Repositories.Interfaces;

public interface IAdminGalleryRepository
{
    public Task<Artwork> AddArtworkAsync(Artwork artwork);

    public Task<List<Artwork>> GetArtworksAsync();
}