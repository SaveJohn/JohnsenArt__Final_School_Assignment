namespace JoArtClassLib.Art.Artwork;

public class UpdateArtworkRequest
{
    
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Artist { get; set; }
    public decimal? Price { get; set; }
    public double? HeightDimension { get; set; }
    public double? WidthDimension { get; set; }
    public bool? ForSale { get; set; }
    
    public virtual List<UpdateImageRequest?> Images { get; set; } = new();
}