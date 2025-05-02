using JoArtClassLib.Configuration.Secrets;
using JohnsenArtAPI.Features.Contact.DTO;
using JohnsenArtAPI.Features.Contact.Interfaces;
using JohnsenArtAPI.Features.Gallery.AdminAccess.Interfaces;
using JohnsenArtAPI.Features.Gallery.Common.Interfaces;
using JohnsenArtAPI.Features.Payments.Interfaces;
using JohnsenArtAPI.Features.Payments.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace JohnsenArtAPI.Features.Payments.Controllers;

public delegate Event StripeEventParser(
    string payload,
    string stripeSignatureHeader,
    string secret);

[ApiController]
[Route("api/webhooks/stripe")]
public class StripeWebhookController : ControllerBase
{
    private readonly ILogger<StripeWebhookController> _logger;
    private readonly StripeConfig _config ;
    private readonly IAdminGalleryService _adminGalleryService;
    private readonly IGalleryService _galleryService;
    private readonly IEmailService _adminMail;
    private readonly IOrderEmailService _orderEmailService;
    private readonly StripeEventParser _parser;
    
    public StripeWebhookController(
        ILogger<StripeWebhookController> logger, 
        IOptions<StripeConfig> config,
        IAdminGalleryService adminGalleryService,
        IGalleryService galleryService,
        IEmailService adminMail,
        IOrderEmailService orderEmailService, 
        StripeEventParser parser)
    {
        _logger = logger;
        _config  = config.Value;
        _adminGalleryService = adminGalleryService;
        _galleryService = galleryService;
        _adminMail = adminMail;
        _orderEmailService = orderEmailService;
        _parser = parser;
    }

    [HttpPost]
    public async Task<IActionResult> Handle()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        
        var endpointSecret = _config.WebhookSecret;
        
        if (string.IsNullOrEmpty(endpointSecret))
        {
            _logger.LogError("Webhook secret is null or empty.");
            return StatusCode(500, "Missing Stripe webhook secret.");
        }
        
        try
        {
            var stripeSignature = Request.Headers["Stripe-Signature"];
            
            /*
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                stripeSignature,
                endpointSecret,
                throwOnApiVersionMismatch: false
            );
            var stripeEvent = _parser(
                json,
                stripeSignature,
                endpointSecret
            );*/
            var stripeEvent = _parser(json, stripeSignature, endpointSecret);

            

            _logger.LogInformation("the webhook endpoint hit: {EventType}", stripeEvent.Type);

            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    var buyerEmail = paymentIntent?.Metadata.GetValueOrDefault("buyer_email");
                    var buyerName = paymentIntent?.Metadata.GetValueOrDefault("buyer_name");
                    var artworkIdRaw = paymentIntent?.Metadata?.GetValueOrDefault("artworkId");
                    

                    if (int.TryParse(artworkIdRaw, out var artworkId))
                    {
                        _logger.LogInformation("Successful payment for artworkId: {artworkId}", artworkId);
                        var artwork = await _galleryService.GetArtworkByIdAsync(artworkId);
                        var updated = await _adminGalleryService.MarkAsSoldAsync(artworkId);
                        var adminEmail = await _adminMail.GetAdminEmailAsync();
                        if (updated)
                        {
                            await _orderEmailService.SendOrderEmailAsync(new EmailMessage
                            {
                                ToEmail = buyerEmail,
                                Subject = "Takk for bestillingen din hos JohnsenArt!",
                                HtmlBody = $@"
                                <p>Hei {buyerName},</p>
                                <p>Takk for at du kjøpte <strong>{artwork.Title}</strong>.</p>
                                <p>Du vil bli kontaktet angående levering.</p>
                                <hr/>
                                <p>Vennlig hilsen,<br/>JohnsenArt</p>",
                                ReplyTo = adminEmail
                            });
                            await _orderEmailService.SendOrderEmailAsync(new EmailMessage
                            {
                                ToEmail = adminEmail,
                                Subject = $"Nytt salg: {artwork.Title}",
                                HtmlBody = $@"
                                <p><strong>{buyerName}</strong> har kjøpt <strong>{artwork.Title}</strong> for {artwork.Price} kr.</p>
                                <p>Kontakt e-post: {buyerEmail}</p>",
                                ReplyTo = buyerEmail 
                            });
                            
                            _logger.LogInformation("Artwork {artworkId}, {artwork.Title} marked as sold.", artworkId, artwork.Title);
                        }
                        else
                            _logger.LogWarning("Artwork {artworkId} could not be updated or was already sold.",
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