using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using JoArtClassLib.AwsSecrets;
using JohnsenArtAPI.Features.Authentication.Services;
using System.Text.Json;

namespace JohnsenArtAPI.Configuration;

public class DbConnectionConfigProvider
{
    private readonly IAmazonSecretsManager _secretsManager;
    private readonly ILogger<AuthService> _logger;

    public DbConnectionConfigProvider(IAmazonSecretsManager secretsManager, ILogger<AuthService> logger)
    {
        _secretsManager = secretsManager;
        _logger = logger;
    }

    public async Task<ConnectionStringSecretConfig> GetConnectionStringConfigAsync()
    {
        _logger.LogInformation("Getting connection string config.");
        var request = new GetSecretValueRequest()
        {
            SecretId = "DbSecrets"
        };

        GetSecretValueResponse response;

        try
        {
            response = await _secretsManager.GetSecretValueAsync(request);
        }
        catch (Exception ex)
        {
            _logger.LogError($"DB Secret not found in AWS Secrets Manager. message: {ex.Message}");
            throw;
        }

        if (string.IsNullOrEmpty(response.SecretString))
        {
            _logger.LogError("DB Secret not found in AWS Secrets Manager.");
            throw new Exception("DB Secret not found in AWS Secrets Manager.");
        }
        
        var config = JsonSerializer.Deserialize<ConnectionStringSecretConfig>(response.SecretString)!;

        if (config == null)
        {
            _logger.LogError("Failed to serialize DB secret config.");
            throw new Exception("Failed to serialize DB secret config.");
        }
        
        return config;
    }
}