using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

public class ImageRequest
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Bilde er påkrevd.")]
    public IFormFile? ImageFile { get; set; }

    public bool IsWallPreview { get; set; }
}