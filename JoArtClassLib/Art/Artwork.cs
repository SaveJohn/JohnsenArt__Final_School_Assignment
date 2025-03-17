using System.ComponentModel.DataAnnotations;
using JoArtClassLib.Art;


namespace JoArtClassLib;

public class Artwork
{
    [Key] public int ArtworkId { get; set; }

    [Required] [MaxLength(100)] public required string ArtTitle { get; set; }
    [MaxLength(2000)] public string? ArtDescription { get; set; }
    [MaxLength(50)] public string? Artist { get; set; }
    
    public decimal? ArtPrice { get; set; }
    public double? HeightDimension { get; set; }
    public double? WidthDimension { get; set; }
    [Required] public bool ForSale { get; set; }
    
    public virtual List<ArtworkImage> Images { get; set; } = new();
    
}