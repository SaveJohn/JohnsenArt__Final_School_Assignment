using JoArtClassLib.Art;
using JoArtClassLib.Payment;
using Stripe;

namespace JohnsenArtAPI.Features.Payments.Interfaces;

public interface IStripeService
{
    Task<PaymentIntent> CreatePaymentIntentAsync(ArtworkResponse artwork, BuyerInfo buyer);
    string GetPublishableKey();
}