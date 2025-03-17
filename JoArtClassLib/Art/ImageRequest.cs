using Microsoft.AspNetCore.Http;

namespace JoArtClassLib.Art;

public class ImageRequest
{
    public required IFormFile ImageFile { get; set; }
    public bool IsWallPreview { get; set; }
}