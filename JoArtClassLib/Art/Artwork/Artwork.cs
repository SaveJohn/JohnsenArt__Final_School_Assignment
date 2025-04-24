using System.ComponentModel.DataAnnotations;
using JoArtClassLib.Art;


namespace JoArtClassLib;

public class Artwork
{
    [Key] public int Id { get; set; }

    [Required] [MaxLength(100)] public required string Title { get; set; }
    [MaxLength(2000)] public string? Description { get; set; }
    [MaxLength(50)] public string? Artist { get; set; }
    [MaxLength(50)]public string? Materials {get; set;}     
    public decimal? Price { get; set; }
    public double? HeightDimension { get; set; }
    public double? WidthDimension { get; set; }
    [Required] public bool ForSale { get; set; }
    
    public bool HomePageRotation { get; set; }
    public virtual List<ArtworkImage?> Images { get; set; } = new();
    
}