using JoArtClassLib.AwsSecrets;

namespace JohnsenArtAPI.Features.Payments.Interfaces;

public interface IStripeConfigProvider
{
    Task<StripeSecretConfig> GetStripeConfigAsync();
}