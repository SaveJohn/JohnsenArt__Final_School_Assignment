using JoArtClassLib.Art;
using JoArtClassLib.Art.Artwork;
using JohnsenArtAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JohnsenArtAPI.Features.Gallery.Admin;

[Authorize]
[Route("admin/api/Gallery")]
[ApiController]
public class AdminGalleryController : ControllerBase 
{
    private readonly IAdminGalleryService _service;
    private readonly ILogger<AdminGalleryController> _logger;

    public AdminGalleryController(
        IAdminGalleryService service, 
        ILogger<AdminGalleryController> logger)
    {
        _service = service;
        _logger = logger;
    }
    
    
    // UPLOAD artwork
    [HttpPost("upload-artwork")]
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

    
    // UPDATE artwork 
    [HttpPut("update-artwork/{id}")]
    public async Task<IActionResult> UpdateArtwork(int id, [FromForm] UpdateArtworkRequest request)
    {
        _logger.LogInformation("Endpoint : EditArtwork called");
        try
        {
            var response = await _service.UpdateArtworkAsync(id, request);

            return response is null
                ? BadRequest("Update Artwork Failed")
                : Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "EditArtwork: Error updating artwork with file(s).");
            return StatusCode(500, "Internal server error.");
        }
    }
    
    // DELETE artwork
    [HttpDelete("delete-artwork/{id}")]
    public async Task<IActionResult> DeleteArtwork(int id)
    {
        _logger.LogInformation("Endpoint : DeleteArtwork called");

        try
        {
            var response = await _service.DeleteArtworkAsync(id);

            return response is null
                ? BadRequest("Delete Artwork Failed")
                : Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteArtwork: Error deleting artwork with file(s).");
            return StatusCode(500, "Internal server error.");
        }
    }
    
}