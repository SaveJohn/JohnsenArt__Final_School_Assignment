using System.Net;

namespace JohnsenArtAPI.Services.Interfaces;

public interface IAdminGalleryService
{
    Task<HttpStatusCode> UploadArtworkAsync(IFormFile imageFile, string bucketName);
}