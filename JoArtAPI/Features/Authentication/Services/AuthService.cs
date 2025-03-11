using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JohnsenArtAPI.Features.Authentication.Interfaces;
using JohnsenArtAPI.Features.Authentication.Models;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace JohnsenArtAPI.Features.Authentication.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest loginRequest)
    {
        if (loginRequest.Email != "admin" || loginRequest.Password != "password")
        {
            return null;
        }

        var user = new UserDTO { Email = loginRequest.Email, AdminId = 1 };
        var token = GenerateJwtToken(user);

        return new AuthResponse { Token = token };
    }

    public string GenerateJwtToken(UserDTO user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.AdminId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Email)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}