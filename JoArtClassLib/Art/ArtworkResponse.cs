namespace JoArtClassLib.Art;

public class ArtworkResponse
{
    public string? ArtTitle { get; set; }
    public string? ArtDescription { get; set; }
    public string? Artist { get; set; } = "Bjarne Johnsen";
    public decimal? ArtPrice { get; set; }
    public double? HeightDimension { get; set; }
    public double? WidthDimension { get; set; }
    public bool ForSale { get; set; }
    
    public virtual List<ImageResponse> Images { get; set; } = new();
}