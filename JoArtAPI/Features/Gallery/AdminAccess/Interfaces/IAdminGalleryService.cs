using JoArtClassLib.Art;
using JoArtClassLib.Art.Artwork;

namespace JohnsenArtAPI.Features.Gallery.AdminAccess.Interfaces;

public interface IAdminGalleryService
{
    Task<ArtworkResponse> UploadArtworkAsync(ArtworkRequest request);

    public Task<ArtworkResponse?> UpdateArtworkAsync(int id, UpdateArtworkRequest request);

    public Task<ArtworkResponse?> DeleteArtworkAsync(int artId);

    public Task<bool> MarkAsSoldAsync(int artworkId);
    
}