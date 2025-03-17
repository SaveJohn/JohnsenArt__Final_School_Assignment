namespace JoArtClassLib.Art;

public class ArtworkRequest
{
    public string ArtTitle { get; set; }
    public string? ArtDescription { get; set; }
    public string Artist { get; set; }
    public decimal? ArtPrice { get; set; }
    public double HeightDimension { get; set; }
    public double WidthDimension { get; set; }
    public bool ForSale { get; set; }
    
    public virtual List<ImageRequest> Images { get; set; } = new();
}