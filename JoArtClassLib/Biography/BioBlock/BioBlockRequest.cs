using JoArtClassLib.About.Enums;

namespace JoArtClassLib.About;

public class BioBlockRequest
{
    public int OrderIndex { get; set; }
    public BlockType Type { get; set; }
    public bool IsPublished { get; set; }
    
    public string? Title { get; set; }
    public string? BodyMarkdown { get; set; }
    
    public ImageTextLayout? Layout { get; set; }
    
    public virtual List<BioImageRequest> Images { get; set; } = new();
}