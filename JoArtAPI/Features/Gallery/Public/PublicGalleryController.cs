using Google.Protobuf.WellKnownTypes;
using JohnsenArtAPI.Features.Gallery.Common.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace JohnsenArtAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PublicGalleryController : ControllerBase
{
    private readonly IGalleryService _galleryService;
    private readonly ILogger<PublicGalleryController> _logger;

    public PublicGalleryController(
        IGalleryService galleryService,
        ILogger<PublicGalleryController> logger)
    {
        _galleryService = galleryService;
        _logger = logger;
    }


    // Get All Artworks
    [HttpGet("artworks")]
    public async Task<IActionResult> GetGalleryArtworks(
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 30,
        [FromQuery] bool? newest = true,
        [FromQuery] bool? forSale = null)
    {
        _logger.LogInformation("Endpoint : GetGalleryArtworks called");
        try
        {
            var response = await _galleryService.GetArtworksAsync(page, perPage, newest, forSale);

            Response.Headers["Cache-Control"] = "public, max-age=31536000, immutable";
            Response.Headers["Content-Type"] = "application/json";


            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetArtworks: Error getting artworks.");
            return StatusCode(500, "Internal server error.");
        }
    }

    // Get Artwork by ID
    [HttpGet("artworks/{artId}")]
    public async Task<IActionResult> GetGalleryArtworkById(int artId)
    {
        _logger.LogInformation("Endpoint : GetGalleryArtworkById called");
        try
        {
            var response = await _galleryService.GetArtworkByIdAsync(artId);

            Response.Headers["Cache-Control"] = "public, max-age=31536000, immutable";
            Response.Headers["Content-Type"] = "application/json";


            return response == null
                ? NotFound("No artwork found")
                : Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving artwork with ID {artId}", artId);
            return StatusCode(500, "An error occurred while retrieving the artwork.");
        }
    }
}