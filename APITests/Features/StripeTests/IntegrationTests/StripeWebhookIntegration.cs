using System.Text;
using JoArtClassLib.Art;
using JoArtClassLib.AwsSecrets;
using JohnsenArtAPI.Features.Contact.Interfaces;
using JohnsenArtAPI.Features.Gallery.Common.Interfaces;
using JohnsenArtAPI.Features.Payments.Controllers;
using JohnsenArtAPI.Features.Payments.Interfaces;
using JohnsenArtAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Stripe;
using Xunit;
using File = System.IO.File;

namespace IntegrationTests.Features.StripeTests.IntegrationTests;
//Integration test checking that the stripewebhook controller proccesses a webhook payload successfully with the help of 
// a fake parser, and to verify that the artwork is marked as sold.

public class StripeWebhookIntegration
{
    [Fact]
    public async Task Handle_ValidStripeWebhookPayload_ProcessesPaymentAndMarksAsSold()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<StripeWebhookController>>();
        var mockConfig = new Mock<IStripeConfigProvider>();
        var mockAdminGallery = new Mock<IAdminGalleryService>();
        var mockGalleryService = new Mock<IGalleryService>();
        var mockEmailService = new Mock<IEmailService>();
        var mockOrderEmail = new Mock<IOrderEmailService>();

        var testArtworkId = 1;
        var artwork = new ArtworkResponse
        {
            Id = testArtworkId,
            Title = "Test Artwork",
            Price = 2500,
            ForSale = true
        };

        mockConfig.Setup(c => c.GetStripeConfigAsync())
            .ReturnsAsync(new StripeSecretConfig { WebhookSecret = "whsec_testsecret" });

        mockGalleryService.Setup(s => s.GetArtworkByIdAsync(testArtworkId)).ReturnsAsync(artwork);
        mockAdminGallery.Setup(s => s.MarkAsSoldAsync(testArtworkId)).ReturnsAsync(true);
        mockEmailService.Setup(s => s.GetAdminEmailAsync()).ReturnsAsync("admin@example.com");


        var json = await File.ReadAllTextAsync("Features/StripeTests/TestData/payment_intent_succeeded.json");


        StripeEventParser fakeParser = (_, _, _) => new Event
        {
            Type = "payment_intent.succeeded",
            Data = new EventData
            {
                Object = new PaymentIntent
                {
                    Metadata = new Dictionary<string, string>
                    {
                        { "artworkId", "1" },
                        { "buyer_email", "buyer@example.com" },
                        { "buyer_name", "John Doe" }
                    }
                }
            }
        };


        var controller = new StripeWebhookController(
            mockLogger.Object,
            mockConfig.Object,
            mockAdminGallery.Object,
            mockGalleryService.Object,
            mockEmailService.Object,
            mockOrderEmail.Object,
            fakeParser
        );


        var context = new DefaultHttpContext();
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(json));
        context.Request.Headers["Stripe-Signature"] = "t=12345,v1=fake";

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = context
        };

        //Act
        var result = await controller.Handle();

        //Assert
        Assert.IsType<OkResult>(result);
        mockAdminGallery.Verify(s => s.MarkAsSoldAsync(testArtworkId), Times.Once);
    }
}