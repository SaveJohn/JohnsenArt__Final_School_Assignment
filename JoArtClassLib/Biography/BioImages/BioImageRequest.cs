using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace JoArtClassLib.About;

public class BioImageRequest
{
    
    public int? Id { get; set; }
    
    public int OrderIndex { get; set; }

    [Required(ErrorMessage = "Bilde er påkrevd.")]
    public IFormFile? ImageFile { get; set; }
    
}