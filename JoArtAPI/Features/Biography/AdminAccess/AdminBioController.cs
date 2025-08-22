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
    
    // Update block
    
    // delete block
}