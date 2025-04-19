using JohnsenArtAPI.Features.Payments.Services;
using JohnsenArtAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace JohnsenArtAPI.Features.Payments.Controllers;

[ApiController]
[Route("api/webhooks/stripe")]
public class StripeWebhookController : ControllerBase
{
    private readonly ILogger<StripeWebhookController> _logger;
    private readonly StripeConfigProvider _stripeConfigProvider ;
    private readonly IAdminGalleryService _adminGalleryService;

    public StripeWebhookController(
        ILogger<StripeWebhookController> logger, 
        StripeConfigProvider config,
        IAdminGalleryService adminGalleryService)
    {
        _logger = logger;
        _stripeConfigProvider  = config;
        _adminGalleryService = adminGalleryService;
    }

    [HttpPost]
    public async Task<IActionResult> Handle()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        
        var config = await _stripeConfigProvider .GetStripeConfigAsync();
        var endpointSecret = config.WebhookSecret;
        
        if (string.IsNullOrEmpty(endpointSecret))
        {
            _logger.LogError("Webhook secret is null or empty.");
            return StatusCode(500, "Missing Stripe webhook secret.");
        }
        
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
                    var artworkIdRaw = paymentIntent?.Metadata?.GetValueOrDefault("artworkId");

                    if (int.TryParse(artworkIdRaw, out var artworkId))
                    {
                        _logger.LogInformation("Successful payment for artworkId: {artworkId}", artworkId);

                        var updated = await _adminGalleryService.MarkAsSoldAsync(artworkId);
                        if (updated)
                            _logger.LogInformation("Artwork {artworkId} marked as sold.", artworkId);
                        else
                            _logger.LogWarning("Artwork {artworkId} could not be updatedor was already sold.",
                                artworkId);
                    }
                    else
                    {
                        _logger.LogWarning("Invalid artworkId metadata: {artworkIdRaw}", artworkIdRaw);
                    }

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