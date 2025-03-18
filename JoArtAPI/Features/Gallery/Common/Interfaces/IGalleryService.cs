using JoArtClassLib.Art;

namespace JohnsenArtAPI.Features.Gallery.Common.Interfaces;

public interface IGalleryService
{
    public Task<IEnumerable<ArtworkResponse?>> GetArtworksAsync(int page, int perPage, bool? newest, bool? forSale);
    public Task<ArtworkResponse?> GetArtworkByIdAsync(int artId);
}