using System.Net;
using JoArtClassLib;
using JoArtClassLib.Art;
using JoArtClassLib.Art.Artwork;

namespace JohnsenArtAPI.Services.Interfaces;

public interface IAdminGalleryService
{
    Task<ArtworkResponse> UploadArtworkAsync(ArtworkRequest request);
    
    public Task<ArtworkResponse?> UpdateArtworkAsync(int id, UpdateArtworkRequest request);
    
    public Task<ArtworkResponse?> DeleteArtworkAsync(int artId);
}