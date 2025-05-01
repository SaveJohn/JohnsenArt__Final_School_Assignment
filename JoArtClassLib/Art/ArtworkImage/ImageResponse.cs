namespace JoArtClassLib.Art;

public class ImageResponse
{
    public required int Id { get; set; }
    public required string FullViewKey { get; set; }
    public required string FullViewUrl { get; set; } // Pre signed URL
    public string ThumbnailKey { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string PreviewKey { get; set; } = string.Empty;
    public string PreviewUrl{ get; set; } = string.Empty;

    
    
}


// 12 10 9 8 6 5 4 3 2 1