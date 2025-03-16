using System.Net;
using JoArtClassLib;
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
    [HttpPost("upload")]
    public async Task<IActionResult> UploadArtwork(
        [FromForm] IFormFile file, 
        [FromForm] ArtworkDTO artworkDto)
    {
        if (file == null || file.Length == 0 || artworkDto == null)
        {
            _logger.LogWarning("UploadArtwork: No file provided.");
            return BadRequest("File is required.");
        }

        try
        {
            var response = await _adminGalleryService.UploadArtworkAsync(file, artworkDto);
            
            _logger.LogInformation($"UploadArtwork: {response}");
            
            return response is null 
                    ? BadRequest("UploadArtwork Failed")
                    : Ok(response);
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UploadArtwork: Error uploading {FileName}", file?.FileName);
            return StatusCode(500, "Internal server error.");
        }
    }

    
    // Edit artwork 
    
    // Delete artwork
    
}