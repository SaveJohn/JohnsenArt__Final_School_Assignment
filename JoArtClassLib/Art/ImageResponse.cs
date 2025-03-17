namespace JoArtClassLib.Art;

public class ImageResponse
{
    public required string ObjectKey { get; set; }
    public required string ImageUrl { get; set; }
    public bool IsWallPreview { get; set; }
    
}