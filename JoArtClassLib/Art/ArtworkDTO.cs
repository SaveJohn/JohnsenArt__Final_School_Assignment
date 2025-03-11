namespace JoArtClassLib;

public class ArtworkDTO
{
    public string ArtTitle { get; set; }
    public string? ArtDescription { get; set; }
    public string Artist { get; set; }
    public decimal? ArtPrice { get; set; }
    public double HeightDimension { get; set; }
    public double WidthDimension { get; set; }
    public bool ForSale { get; set; }
    
    public List<ArtworkImageDTO> Images { get; set; } = new List<ArtworkImageDTO>();
}