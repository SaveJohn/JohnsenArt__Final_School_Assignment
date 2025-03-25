using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace JohnsenArtAPI.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddJtwAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Retrieving and checking whether jwt key is present
        var jwtKey64 = configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey64))
        {
            throw new InvalidOperationException("The JWT key is missing, check user secrets/appsettings.json.");
        }
        
        var key = new SymmetricSecurityKey(Convert.FromBase64String(configuration["Jwt:Key"]!));
        
        //JWT Set up
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                ValidateLifetime = true
            };
        });
        return services;
    }
}