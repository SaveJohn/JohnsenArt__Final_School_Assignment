using JoArtClassLib.Art;
using JohnsenArtAPI.Features.Payments.Interfaces;
using Stripe;

namespace JohnsenArtAPI.Features.Payments.Services;

public class StripeService : IStripeService
{
    private readonly StripeConfigProvider _stripeConfigProvider ;
    private readonly ILogger<StripeService> _logger;

    public StripeService(StripeConfigProvider config, ILogger<StripeService> logger)
    {
        _stripeConfigProvider  = config;
        _logger = logger;
    }

    public async Task<PaymentIntent> CreatePaymentIntentAsync(ArtworkResponse artwork)
    {

        var config = await _stripeConfigProvider .GetStripeConfigAsync();
        StripeConfiguration.ApiKey = config.SecretKey;
        
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)((artwork.Price ?? 0) * 100),
            Currency = "nok",
            PaymentMethodTypes = new List<string> { "card", "klarna" },
            Metadata = new Dictionary<string, string>
            {
                { "artworkId", artwork.Id.ToString() },
                { "title", artwork.Title },
            }
        };

        var service = new PaymentIntentService();
        var intent = await service.CreateAsync(options);
        _logger.LogInformation("Intent payment created with id {Id}", intent.Id);

        return intent;
    }
}