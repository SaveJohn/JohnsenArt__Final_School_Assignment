using JoArtClassLib;
using JoArtClassLib.Enums;

namespace JoArtDataLayer.Repositories.Interfaces;

public interface IGalleryRepository
{
    public Task<IEnumerable<Artwork?>> GetArtworksAsync(int page, int perPage, GallerySort sort, bool? forSale);
    public Task<Artwork?> GetArtworkByIdAsync(int artId);
}