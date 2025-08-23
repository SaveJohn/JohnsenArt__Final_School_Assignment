using System.ComponentModel.DataAnnotations;
using JoArtClassLib.About.Enums;

namespace JoArtClassLib.About;

public class BioBlock
{
    [Key] public int Id { get; set; }
    public int OrderIndex { get; set; }
    public BlockType Type { get; set; }
    public bool IsPublished { get; set; }
    
    public string? Title { get; set; }
    public string? BodyMarkdown { get; set; }
    
    public ImageTextLayout? Layout { get; set; }
    
    public virtual List<BioImage?> Images { get; set; } = new();
}