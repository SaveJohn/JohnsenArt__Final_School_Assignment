using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JoArtClassLib.Art;

public class ArtworkImage
{
    [Key] public int ImageId { get; set; }
    [Required] public required string ObjectKey  { get; set; } // S3 file path
    public bool IsWallPreview { get; set; }  

    [ForeignKey(nameof(ArtworkId))]
    public int ArtworkId { get; set; }
    public virtual Artwork? Artwork { get; set; }
}