namespace JoArtClassLib.Art;

using System.ComponentModel.DataAnnotations;

public class ArtworkRequest
{
    [Required(ErrorMessage = "Tittel er påkrevd.")]
    public string Title { get; set; }

    public string? Description { get; set; }

    public string Artist { get; set; } = "Bjarne Johnsen";

    [Range(0, double.MaxValue, ErrorMessage = "Pris må være et positivt tall.")]
    public decimal? Price { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Høyde må være et positivt tall.")]
    public double? HeightDimension { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Bredde må være et positivt tall.")]
    public double? WidthDimension { get; set; }

    public bool ForSale { get; set; }
    
    public virtual List<ImageRequest> Images { get; set; } = new();
}
