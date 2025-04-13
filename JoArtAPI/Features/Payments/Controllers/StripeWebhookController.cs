using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace JohnsenArtAPI.Features.Payments.Controllers;

[ApiController]
[Route("api/webhooks/stripe")]
public class StripeWebhookController : ControllerBase
{
    private readonly ILogger<StripeWebhookController> _logger;
    private readonly IConfiguration _config;

    public StripeWebhookController(ILogger<StripeWebhookController> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    [HttpPost]
    public async Task<IActionResult> Handle()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var endpointSecret = _config["Stripe:WebhookSecret"];

        try
        {
            var stripeSignature = Request.Headers["Stripe-Signature"];
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                stripeSignature,
                endpointSecret,
                throwOnApiVersionMismatch: false
            );


            _logger.LogInformation("the webhook endpoint hit: {EventType}", stripeEvent.Type);

            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    var artworkId = paymentIntent?.Metadata?.GetValueOrDefault("artworkId");

                    _logger.LogInformation(" payment successfull for artworkId: {artworkId}", artworkId);
                    break;


                default:
                    _logger.LogInformation("Unhandled event type: {EventType}", stripeEvent.Type);
                    break;
            }

            return Ok();
        }
        catch (StripeException se)
        {
            _logger.LogError(se, "Stripe Exception: {Message}", se.Message);
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "general Exception: {Message}", ex.Message);
            return BadRequest();
        }
    }
}