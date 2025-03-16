using JohnsenArtAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JohnsenArtAPI.Controllers;


[Route("admin/api/[controller]")]
public class AdminGalleryController : ControllerBase 
{
    private readonly IAdminGalleryService _adminGalleryService;
    private readonly ILogger<AdminGalleryController> _logger;

    public AdminGalleryController(
        IAdminGalleryService adminGalleryService, 
        ILogger<AdminGalleryController> logger)
    {
        _adminGalleryService = adminGalleryService;
        _logger = logger;
    }
    
    // Get artwork
    
    // Upload artwork
    [HttpPost]
    public async Task<IResult> UploadArtwork([FromForm]IFormFile imageFile, string bucketName)
    {
        var response = await _adminGalleryService.UploadArtworkAsync(imageFile, bucketName);
        return Results.Ok(response);
    }
    
    // Edit artwork 
    
    // Delete artwork
    
}