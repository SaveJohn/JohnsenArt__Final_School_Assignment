namespace JoArtClassLib.Art;

public class ArtworkResponse
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Artist { get; set; } 
    public string? Materials {get; set;}
    public decimal? Price { get; set; }
    public double? HeightDimension { get; set; }
    public double? WidthDimension { get; set; }
    public bool ForSale { get; set; }
    public bool homePageRotation { get; set; }
    public virtual List<ImageResponse> Images { get; set; } = new();
}