using JohnsenArtAPI.Features.Authentication.Interfaces;
using JohnsenArtAPI.Features.Authentication.Models;
using Microsoft.AspNetCore.Mvc;

namespace JohnsenArtAPI.Features.Authentication;

[ApiController]
[Route("admin/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        _logger.LogInformation("Auth Controller - endpoint login hit");

        var response = await _authService.LoginAsync(loginRequest);
        if (response == null || !response.WasSuccessful)
        {
            return Unauthorized(new
            {
                ErrorMessage = response?.ErrorMessage ?? "Login request failed"
            });
        }

        return Ok(new { Token = response.Token });
        
    }
}