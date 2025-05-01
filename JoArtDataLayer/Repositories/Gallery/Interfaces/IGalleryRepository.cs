using JoArtClassLib;
using JoArtClassLib.Art.Artwork;
using JoArtClassLib.Enums;

namespace JoArtDataLayer.Repositories.Interfaces;

public interface IGalleryRepository
{
    public Task<IEnumerable<Artwork?>> GetArtworksAsync(int page, int perPage, GallerySort sort, GalleryFilter filter);
    public Task<Artwork?> GetArtworkByIdAsync(int artId);
    
    public Task<Neighbors> GetGalleryNeighborsAsync(int artId, GallerySort sort, GalleryFilter filter);

    public Task<IEnumerable<string?>> GetRotationObjectKeys();
}