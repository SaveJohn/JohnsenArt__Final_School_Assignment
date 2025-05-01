using JoArtClassLib.AwsSecrets;
using JohnsenArtAPI.AWS.Configuration;
using JohnsenArtAPI.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace JohnsenArtAPI.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        JwtSecretConfig jwtConfig)
    {
        
        // var key = new SymmetricSecurityKey(Convert.FromBase64String(configuration["Jwt:Key"]!));
        var key = new SymmetricSecurityKey(Convert.FromBase64String(jwtConfig.Key));
        
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
                ValidIssuer = jwtConfig.Issuer,
                ValidAudience = jwtConfig.Audience,
                ValidateLifetime = true
            };
        });
        return services;
    }
    
    public static void AddAmazonSecretsManager(this IConfigurationBuilder configurationBuilder, 
        string region,
        string secretName)
    {
        var configurationSource = 
            new AmazonSecretsManagerConfigurationSource(region, secretName);

        configurationBuilder.Add(configurationSource);
    }
}