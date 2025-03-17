using System.Net;
using JoArtClassLib;
using JoArtClassLib.Art;

namespace JohnsenArtAPI.Services.Interfaces;

public interface IAdminGalleryService
{
    Task<ArtworkResponse> UploadArtworkAsync(ArtworkRequest request);
}