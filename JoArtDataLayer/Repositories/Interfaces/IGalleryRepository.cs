using JoArtClassLib;

namespace JoArtDataLayer.Repositories.Interfaces;

public interface IGalleryRepository
{
    public Task<IEnumerable<Artwork?>> GetArtworksAsync(int page, int perPage, bool? newest, bool? forSale);
    public Task<Artwork?> GetArtworkByIdAsync(int artId);
}