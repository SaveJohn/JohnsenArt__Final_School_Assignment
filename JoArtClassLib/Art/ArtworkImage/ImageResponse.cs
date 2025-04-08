namespace JoArtClassLib.Art;

public class ImageResponse
{
    public required int Id { get; set; }
    public required string ObjectKey { get; set; }
    public required string ImageUrl { get; set; } // Pre signed URL
    public string ThumbnailKey { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;

    
    public bool IsWallPreview { get; set; }
    
}


// 12 10 9 8 6 5 4 3 2 1