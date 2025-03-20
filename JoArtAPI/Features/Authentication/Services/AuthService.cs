using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BCrypt.Net;
using JoArtDataLayer;
using Microsoft.EntityFrameworkCore;
using System.Text;
using JohnsenArtAPI.Features.Authentication.Interfaces;
using JohnsenArtAPI.Features.Authentication.Models;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace JohnsenArtAPI.Features.Authentication.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly JoArtDbContext _dbContext;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IConfiguration configuration, JoArtDbContext dbContext, ILogger<AuthService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest loginRequest)
    {
        var admin = await _dbContext.Admins
            .FirstOrDefaultAsync(a => a.Email == loginRequest.Email);

        if (admin == null)
        {
            return null;
        }

        var user = new UserDTO { Email = loginRequest.Email, AdminId = 1 };
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginRequest.Password, admin.HashedPassword);

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
        var keyBytes = Convert.FromBase64String(_configuration["JWT:Key"]!);
        var key = new SymmetricSecurityKey(keyBytes);
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