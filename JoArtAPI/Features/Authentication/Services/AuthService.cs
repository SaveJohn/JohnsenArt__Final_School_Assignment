using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BCrypt.Net;
using JoArtDataLayer;
using Microsoft.EntityFrameworkCore;
using System.Text;
using JoArtClassLib.AwsSecrets;
using JohnsenArtAPI.Features.Authentication.Interfaces;
using JohnsenArtAPI.Features.Authentication.Models;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace JohnsenArtAPI.Features.Authentication.Services;

public class AuthService : IAuthService
{
    private readonly JwtSecretConfig _jwtConfig;
    private readonly JoArtDbContext _dbContext;
    private readonly ILogger<AuthService> _logger;

    public AuthService(JwtSecretConfig jwtConfig, JoArtDbContext dbContext, ILogger<AuthService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
        _jwtConfig = jwtConfig;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest loginRequest)
    {
        _logger.LogInformation("Login request received for email: {Email}", loginRequest.Email);


        var admin = await _dbContext.Admins
            .FirstOrDefaultAsync(a => a.Email == loginRequest.Email);

        if (admin == null)
        {
            _logger.LogWarning("Login request failed: No user found with email/username '{Email}'", loginRequest.Email);
            return new AuthResponse
            {
                ErrorMessage = "Invalid email/username."
            };
        }

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginRequest.Password, admin.HashedPassword);

        if (!isPasswordValid)
        {
            _logger.LogWarning("Login request failed: Invalid password entered.");
            return new AuthResponse
            {
                ErrorMessage = "Invalid password entered."
            };
        }

        var user = new UserDTO
        {
            AdminId = admin.AdminId,
            Email = admin.Email,
            Name = admin.Name,
        };
        var token = GenerateJwtToken(user);

        return new AuthResponse { Token = token };
    }

    public string GenerateJwtToken(UserDTO user)
    {
        var keyBytes = Convert.FromBase64String(_jwtConfig.Key);
        var key = new SymmetricSecurityKey(keyBytes);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.AdminId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Email)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}