using JohnsenArtAPI.Features.Biography.AdminAccess.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JohnsenArtAPI.Features.Biography.AdminAccess;

public class AdminBioController : ControllerBase
{
    private readonly ILogger<AdminBioController> _logger;
    private readonly IAdminBioService _service;

    public AdminBioController( ILogger<AdminBioController> logger, IAdminBioService service)
    {
        _logger = logger;
        _service = service;
    }
    
    // Get draft
    
    // Publish draft
    
    // Add new block
    [HttpPost("upload-bioblock")]
    public async Task<IActionResult> UploadBioBlock([FromForm] BioBlockRequest request)
    {
        _logger.LogInformation("Endpoint : UploadArtwork called");
        
        if (request == null || request.Images == null || request.Images.Count == 0)
        {
            _logger.LogWarning("UploadArtwork: No file or image details provided.");
            return BadRequest("File(s) are required.");
        }

        try
        {
            var response = await _service.UploadArtworkAsync(request);
            
            return response is null 
                ? BadRequest("Upload Artwork Failed") 
                : Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UploadArtwork: Error uploading artwork with file(s).");
            return StatusCode(500, "Internal server error.");
        }
    }
    
    // Update block
    
    // delete block
}