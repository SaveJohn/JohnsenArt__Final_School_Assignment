using JoArtClassLib.Art;
using JoArtClassLib.Art.Artwork;
using JoArtClassLib.Enums;

namespace JohnsenArtAPI.Features.Gallery.Common.Interfaces;

public interface IGalleryService
{
    public Task<IEnumerable<ArtworkResponse?>> GetArtworksAsync(int page, int perPage, GallerySort sort, GalleryFilter filter);
    public Task<ArtworkResponse?> GetArtworkByIdAsync(int artId);
    
    public Task<NeighborsResponse> GetGalleryNeighborsAsync(int artId, GallerySort sort, GalleryFilter filter);
    
    public Task<IEnumerable<ImageResponse?>> GetRotationImagesAsync();
    
    public Task<IEnumerable<string?>> GetRotationUrls();
}