using System.ComponentModel.DataAnnotations;


namespace JoArtClassLib;

public class Listing
{
    [Key] public int ListingId { get; set; }

    [Required] [MaxLength(50)] public string ArtTitle { get; set; }
    [MaxLength(50)] public string? ArtDescription { get; set; }
    [Required] [MaxLength(50)] public string ArtAuthor { get; set; }
    [MaxLength(50)] public decimal? ArtPrice { get; set; }
    [MaxLength(50)] public Dictionary<string, double> Dimensions { get; set; }
    [Required] public bool ForSale { get; set; }
    
    public virtual Order Order { get; set; }
}