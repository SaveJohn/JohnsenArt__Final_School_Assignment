using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JoArtClassLib;

public class Order
{
    [Key] public int OrderId { get; set; }

    [ForeignKey("ListingId")] public int ArtworkId { get; set; }

    [Required] [MaxLength(256)] public string Name { get; set; } = string.Empty;
    [Required] [MaxLength(256)] public string Address { get; set; } = string.Empty;
    [Required] [MaxLength(256)] public string PhoneNumber { get; set; } = string.Empty;
    [Required] [MaxLength(256)] public string Email { get; set; } = string.Empty;

    // Navigation Properties
    public virtual Artwork Artwork { get; set; }
}