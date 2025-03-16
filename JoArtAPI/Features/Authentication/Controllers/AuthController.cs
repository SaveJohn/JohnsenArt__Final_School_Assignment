using JohnsenArtAPI.Features.Authentication.Interfaces;
using JohnsenArtAPI.Features.Authentication.Models;
using Microsoft.AspNetCore.Mvc;

namespace JohnsenArtAPI.Features.Authentication.Controllers;

[ApiController]
[Route("admin/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        Console.WriteLine("Auth controller - endpoint login hit");

        var response = await _authService.LoginAsync(loginRequest);
        if (response == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        return Ok(response);
    }
}