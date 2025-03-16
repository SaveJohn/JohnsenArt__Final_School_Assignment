using System.Net;
using JoArtClassLib;

namespace JohnsenArtAPI.Services.Interfaces;

public interface IAdminGalleryService
{
    Task<ArtworkDTO> UploadArtworkAsync(IFormFile imageFile, ArtworkDTO artworkDto);
}