namespace JoArtClassLib.AwsSecrets;

public class StripeSecretConfig
{
    public string SecretKey { get; set; }
    public string PublishableKey { get; set; }
    public string WebhookSecret { get; set; }
    public string AdminEmail { get; set; }
}