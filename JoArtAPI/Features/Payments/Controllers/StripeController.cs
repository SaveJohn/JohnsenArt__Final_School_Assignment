using JoArtClassLib.Payment;
using JohnsenArtAPI.Features.Gallery.Common.Interfaces;
using JohnsenArtAPI.Features.Payments.Interfaces;
using JohnsenArtAPI.Features.Payments.Services;
using Microsoft.AspNetCore.Mvc;

namespace JohnsenArtAPI.Features.Payments.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StripeController : ControllerBase
{
    private readonly IStripeService _stripeService;
    private readonly IGalleryService _galleryService;
    private readonly ILogger<StripeController> _logger;

    public StripeController(IStripeService stripeService,
        IGalleryService galleryService,
        ILogger<StripeController> logger)
    {
        _stripeService = stripeService;
        _galleryService = galleryService;
        _logger = logger;
    }

    [HttpPost("create-intent/{artworkId}")]
    public async Task<IActionResult> CreatePaymentIntent(int artworkId, [FromBody] BuyerInfo buyer)
    {
        _logger.LogInformation($"Creating payment intent for {artworkId}");

        var artwork = await _galleryService.GetArtworkByIdAsync(artworkId);
        if (artwork == null || artwork.Price == null || artwork.ForSale == false)
        {
            return BadRequest("Chosen artwork is not available for sale.");
        }

        var intent = await _stripeService.CreatePaymentIntentAsync(artwork, buyer);
        return Ok(new { clientSecret = intent.ClientSecret });
    }

    [HttpGet("publishable-key")]
    public async Task<IActionResult> GetPublishableKey([FromServices] StripeConfigProvider stripeProvider)
    {
        var config = await stripeProvider.GetStripeConfigAsync();
        return Ok(new { publishableKey = config.PublishableKey });
    }
}