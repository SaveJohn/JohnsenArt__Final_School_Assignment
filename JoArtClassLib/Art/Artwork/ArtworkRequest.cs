namespace JoArtClassLib.Art;

public class ArtworkRequest
{
    
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Artist { get; set; } = "Bjarne Johnsen";
    public decimal? Price { get; set; }
    public double? HeightDimension { get; set; }
    public double? WidthDimension { get; set; }
    public bool ForSale { get; set; }
    
    public virtual List<ImageRequest> Images { get; set; } = new();
}