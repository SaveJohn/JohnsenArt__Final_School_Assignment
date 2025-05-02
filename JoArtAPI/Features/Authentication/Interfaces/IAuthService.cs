using JohnsenArtAPI.Features.Authentication.Models;

namespace JohnsenArtAPI.Features.Authentication.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest loginRequest);
    string GenerateJwtToken (AdminDTO admin);
}