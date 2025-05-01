using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System.Text.Json;
using JoArtClassLib.Configuration.Secrets;

namespace JohnsenArtAPI.Features.Payments.Services;

public class StripeConfigProvider
{
    private readonly IAmazonSecretsManager _secretsManager;
    private readonly ILogger<StripeConfigProvider> _logger;

    public StripeConfigProvider(IAmazonSecretsManager secretsManager, ILogger<StripeConfigProvider> logger)
    {
        _secretsManager = secretsManager;
        _logger = logger;
    }
    
    
    // Getting Stripe secrets from AWS Secrets Manager
    public async Task<StripeConfig> GetStripeConfigAsync()
    {
        var request = new GetSecretValueRequest
        {
            SecretId = "StripeSecrets"
        };
        
        GetSecretValueResponse response;
        
        try
        {
            response = await _secretsManager.GetSecretValueAsync(request);
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"Stripe Secret not found in AWS Secrets Manager. message: {ex.Message}");
            throw;
        }
        
        if (string.IsNullOrEmpty(response.SecretString))
        {
            _logger.LogInformation("Stripe Secret not found in AWS Secrets Manager.");
            throw new Exception($"Stripe Secret not found in AWS Secrets Manager.");
        }
        
        var config = JsonSerializer.Deserialize<StripeConfig>(response.SecretString);
        
        if (config is null)
        {
            _logger.LogInformation("Failed to serialize stripe secret config.");
            throw new Exception($"Failed to serialize stripe secret config.");
        }

        return config;
    }
}