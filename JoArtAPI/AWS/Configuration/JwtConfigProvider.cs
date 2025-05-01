using System.Text.Json;
using Amazon.S3;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using JoArtClassLib.AwsSecrets;

namespace JohnsenArtAPI.Configuration;

public class JwtConfigProvider
{
    private readonly IAmazonSecretsManager _secretsManager;
    private readonly ILogger<JwtConfigProvider> _logger;

    public JwtConfigProvider(IAmazonSecretsManager secretsManager, ILogger<JwtConfigProvider> logger)
    {
        _secretsManager = secretsManager;
        _logger = logger;
    }

    public async Task<JwtSecretConfig> GetJwtConfigAsync()
    {
        _logger.LogInformation("Getting jwt config.");
        var request = new GetSecretValueRequest
        {
            SecretId = "JwtSecrets",
        };
        
        GetSecretValueResponse response;
        
        try
        {
            response = await _secretsManager.GetSecretValueAsync(request);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Jwt Secret not found in AWS Secrets Manager. message: {ex.Message}");
            throw;
        }
        
        if (string.IsNullOrEmpty(response.SecretString))
        {
            _logger.LogError("Jwt Secret not found in AWS Secrets Manager.");
            throw new Exception("Jwt Secret not found in AWS Secrets Manager.");
        }
        
        var config = JsonSerializer.Deserialize<JwtSecretConfig>(response.SecretString)!;;
        
        if (config is null)
        {
            _logger.LogError("Failed to serialize jwt secret config.");
            throw new Exception("Failed to serialize jwt secret config.");
        }

        return config;
    }
}