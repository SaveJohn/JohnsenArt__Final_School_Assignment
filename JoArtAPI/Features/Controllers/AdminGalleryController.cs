using System.Net;
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
    public async Task<IActionResult> UploadArtwork([FromForm] IFormFile imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            _logger.LogWarning("UploadArtwork: No file provided.");
            return BadRequest("File is required.");
        }

        try
        {
            var response = await _adminGalleryService.UploadArtworkAsync(imageFile);

            if (response == HttpStatusCode.OK || response == HttpStatusCode.Created)
            {
                _logger.LogInformation("UploadArtwork: File {FileName} uploaded successfully.", imageFile.FileName);
                return Ok(new { Message = "Upload successful", FileName = imageFile.FileName });
            }

            _logger.LogError("UploadArtwork: Upload failed for {FileName}, StatusCode: {StatusCode}", 
                imageFile.FileName, response);
            return StatusCode((int)response, "Upload failed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UploadArtwork: Error uploading {FileName}", imageFile?.FileName);
            return StatusCode(500, "Internal server error.");
        }
    }

    
    // Edit artwork 
    
    // Delete artwork
    
}