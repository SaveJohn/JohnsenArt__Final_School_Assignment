using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JoArtClassLib.Art;

public class Image
{
    [Key] public int Id { get; set; }
    [Required] public required string ObjectKey { get; set; } // S3 file path
    
    public string ThumbnailKey { get; set; } = string.Empty;
    public string PreviewKey { get; set; } = string.Empty;

    [ForeignKey(nameof(Id))] public int ArtworkId { get; set; }
    public virtual JoArtClassLib.Artwork? Artwork { get; set; }
}