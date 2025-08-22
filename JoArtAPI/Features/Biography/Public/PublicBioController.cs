using JohnsenArtAPI.Features.Biography.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JohnsenArtAPI.Features.Biography.Public;

public class PublicBioController : ControllerBase
{
    private readonly ILogger<PublicBioController> _logger;
    private readonly IBioService _service;

    public PublicBioController(ILogger<PublicBioController> logger, IBioService service)
    {
        _logger = logger;
        _service = service;
    }
    
    // Get block
    
}