using JoArtClassLib.Art;
using JoArtClassLib.Enums;

namespace JohnsenArtAPI.Features.Gallery.Common.Interfaces;

public interface IGalleryService
{
    public Task<IEnumerable<ArtworkResponse?>> GetArtworksAsync(int page, int perPage, GallerySort sort, GalleryFilter filter);
    public Task<ArtworkResponse?> GetArtworkByIdAsync(int artId);
}