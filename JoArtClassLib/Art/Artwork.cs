using System.ComponentModel.DataAnnotations;


namespace JoArtClassLib;

public class Artwork
{
    [Key] public int ArtworkId { get; set; }

    [Required] [MaxLength(50)] public string ArtTitle { get; set; }
    [MaxLength(50)] public string? ArtDescription { get; set; }
    [Required] [MaxLength(50)] public string ArtAuthor { get; set; }
    public decimal? ArtPrice { get; set; }
    public double HeightDimension { get; set; }
    public double WidthDimension { get; set; }
    [Required] public bool ForSale { get; set; }
    
    public virtual Order Order { get; set; }
}