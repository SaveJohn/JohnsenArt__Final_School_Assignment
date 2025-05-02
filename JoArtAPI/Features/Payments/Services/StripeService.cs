using JoArtClassLib.Art;
using JoArtClassLib.Configuration.Secrets;
using JoArtClassLib.Payment;
using JohnsenArtAPI.Features.Payments.Interfaces;
using Microsoft.Extensions.Options;
using Stripe;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace JohnsenArtAPI.Features.Payments.Services;

public class StripeService : IStripeService
{
    private readonly StripeConfig _config;
    private readonly ILogger<StripeService> _logger;

    public StripeService(IOptions<StripeConfig> config, ILogger<StripeService> logger)
    {
        _config = config.Value;
        _logger = logger;
    }

    public async Task<PaymentIntent> CreatePaymentIntentAsync(ArtworkResponse artwork, BuyerInfo buyer)
    {
        StripeConfiguration.ApiKey = _config.SecretKey;

        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)((artwork.Price ?? 0) * 100),
            Currency = "nok",
            PaymentMethodTypes = new List<string> { "card", "klarna" },
            Metadata = new Dictionary<string, string>
            {
                { "artworkId", artwork.Id.ToString() },
                { "title", artwork.Title },
                {"buyer_email", buyer.Email },
                {"buyer_name", buyer.FullName},
                {"buyer_phone", buyer.PhoneNumber},
                {"buyer_delivery", buyer.DeliveryMethod},
                {"buyer_address", $"{buyer.AddressLine} {buyer.PostalCode} {buyer.City}"}
            }
        };

        var service = new PaymentIntentService();
        var intent = await service.CreateAsync(options);
        _logger.LogInformation("Intent payment created with id {Id}", intent.Id);

        return intent;
    }

    public string GetPublishableKey()
    {
        return _config.PublishableKey;
    }
}