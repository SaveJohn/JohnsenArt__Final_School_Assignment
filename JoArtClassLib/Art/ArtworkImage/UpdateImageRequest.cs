using Microsoft.AspNetCore.Http;

namespace JoArtClassLib.Art;

public class UpdateImageRequest
{
    public int Id { get; set; }
    public IFormFile? ImageFile { get; set; }
    public bool IsWallPreview { get; set; }
    
    public int ArtworkId { get; set; }
    
}