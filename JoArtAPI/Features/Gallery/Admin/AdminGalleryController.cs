using System.Net;
using JoArtClassLib;
using JoArtClassLib.Art;
using JohnsenArtAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JohnsenArtAPI.Controllers;


[Route("admin/api/[controller]")]
[ApiController]
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
    public async Task<IActionResult> UploadArtwork([FromForm] ArtworkRequest request)
    {
        _logger.LogInformation("Endpoint : UploadArtwork called");
        
        if (request == null || request.Images == null || request.Images.Count == 0)
        {
            _logger.LogWarning("UploadArtwork: No file or image details provided.");
            return BadRequest("File(s) are required.");
        }

        try
        {
            var response = await _adminGalleryService.UploadArtworkAsync(request);
            
            return response is null 
                ? BadRequest("UploadArtwork Failed") 
                : Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UploadArtwork: Error uploading artwork with file(s).");
            return StatusCode(500, "Internal server error.");
        }
    }

    
    // Edit artwork 
    
    // Delete artwork
    
}