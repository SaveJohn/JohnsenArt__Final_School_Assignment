using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace JoArtClassLib.About;

public class UpdateBioImageRequest
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Bilde er påkrevd.")]
    public IFormFile? ImageFile { get; set; }
    
    public int BlockId { get; set; }
}