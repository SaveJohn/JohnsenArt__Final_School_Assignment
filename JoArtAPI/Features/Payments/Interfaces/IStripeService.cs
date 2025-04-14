using JoArtClassLib.Art;
using Stripe;

namespace JohnsenArtAPI.Features.Payments.Interfaces;

public interface IStripeService
{
    Task<PaymentIntent> CreatePaymentIntentAsync(ArtworkResponse artwork);
}