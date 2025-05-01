using Google.Protobuf.WellKnownTypes;
using JoArtClassLib.Art.Artwork;
using JoArtClassLib.Enums;
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
        [FromQuery] GallerySort sort = GallerySort.Newest,
        [FromQuery] GalleryFilter filter = GalleryFilter.All)
    {
        _logger.LogInformation("Endpoint : GetGalleryArtworks called");
        try
        {
            var response = await _galleryService.GetArtworksAsync(page, perPage, sort, filter);

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

    [HttpGet("artworks/{artId}/neighbors")]
    public async Task<IActionResult> GetGalleryNeighbors(int artId, GallerySort sort, GalleryFilter filter)
    {
        _logger.LogInformation("Endpoint : GetGalleryNeighbors called");
        try
        {
            _logger.LogInformation("Art Id: {artId}, sort: {sort}, filter: {filter}", artId, sort, filter);
            
            var neighbors = await _galleryService.GetGalleryNeighborsAsync(artId, sort, filter);
        
            return Ok(neighbors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving neighbors for artwork ID {artId}", artId);
            return StatusCode(500, "An error occurred while retrieving neighbors.");
        }
        
    }
    
    [HttpGet("homePageRotation")]
    public async Task<IActionResult> GetHomePageRotation()
    {
        _logger.LogInformation("Endpoint : GetHomePageRotation called");
        try
        {
            var images = await _galleryService.GetRotationImagesAsync();
            return Ok(images);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving rotation urls.");
            return StatusCode(500, "An error occurred while retrieving the rotation urls.");
        }
    }

    [HttpGet("homePageRotationURL")]
    public async Task<IActionResult> GetHomePageRotationURL()
    {
        _logger.LogInformation("Endpoint : GetHomePageRotation called");
        try
        {
            var urls = await _galleryService.GetRotationUrls();
            return Ok(urls);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving rotation urls.");
            return StatusCode(500, "An error occurred while retrieving the rotation urls.");
        }
    }
}