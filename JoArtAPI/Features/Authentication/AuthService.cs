using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using JoArtClassLib.Configuration.Secrets;
using JoArtDataLayer.Repositories.Interfaces;
using JohnsenArtAPI.Features.Authentication.Interfaces;
using JohnsenArtAPI.Features.Authentication.Models;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace JohnsenArtAPI.Features.Authentication;

public class AuthService : IAuthService
{
    private readonly JwtConfig _jwtConfig;
    private readonly IAdminUserRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        JwtConfig jwtConfig, 
        IAdminUserRepository repository, 
        IMapper mapper,
        ILogger<AuthService> logger)
    {
        
        _logger = logger;
        _jwtConfig = jwtConfig;
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest loginRequest)
    {
        _logger.LogInformation("Login request received for email: {Email}", loginRequest.Email);
        
        var admin = await _repository.GetAdmin(loginRequest.Email);

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
        
        var user = new AdminDTO
        {
            AdminId = admin.AdminId,
            Email = admin.Email,
            Name = admin.Name,
        };
        var token = GenerateJwtToken(user);

        return new AuthResponse { Token = token };
    }

    public string GenerateJwtToken(AdminDTO admin)
    {
        var keyBytes = Convert.FromBase64String(_jwtConfig.Key);
        var key = new SymmetricSecurityKey(keyBytes);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, admin.AdminId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, admin.Email)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(3),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}