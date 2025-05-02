using System.Text;
using IntegrationTests.Features.StripeTests.IntegrationTests.Helpers;
using JoArtClassLib.Art;
using JoArtClassLib.Configuration.Secrets;
using JohnsenArtAPI.Features.Contact.Interfaces;
using JohnsenArtAPI.Features.Gallery.AdminAccess.Interfaces;
using JohnsenArtAPI.Features.Gallery.Common.Interfaces;
using JohnsenArtAPI.Features.Payments.Controllers;
using JohnsenArtAPI.Features.Payments.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Stripe; 
using Xunit;
using File = System.IO.File;

namespace IntegrationTests.Features.StripeTests.IntegrationTests;
//Integration test checking that the stripewebhook controller proccesses a webhook payload successfully with the help of 
// a fake parser, and to verify that the artwork is marked as sold.

// OBS for this to work you have to uncomment a fakestripe
public class StripeWebhookIntegration
{
    [Fact]
    public async Task Handle_ValidStripeWebhookPayload_ProcessesPaymentAndMarksAsSold()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<StripeWebhookController>>();
        var mockAdminGallery = new Mock<IAdminGalleryService>();
        var mockGalleryService = new Mock<IGalleryService>();
        var mockEmailService = new Mock<IEmailService>();
        var mockOrderEmail = new Mock<IOrderEmailService>();
        
        var stripeConfig = new StripeConfig {
            SecretKey      = "sk_test_xxx",
            PublishableKey = "pk_test_xxx",
            WebhookSecret  = "whsec_testsecret"
        };
        var options = Options.Create(stripeConfig);
        
        var testArtworkId = 1;
        var artwork = new ArtworkResponse
        {
            Id = testArtworkId,
            Title = "Test Artwork",
            Price = 2500,
            ForSale = true
        };
        

        mockGalleryService.Setup(s => s.GetArtworkByIdAsync(testArtworkId)).ReturnsAsync(artwork);
        mockAdminGallery.Setup(s => s.MarkAsSoldAsync(testArtworkId)).ReturnsAsync(true);
        mockEmailService.Setup(s => s.GetAdminEmailAsync()).ReturnsAsync("admin@example.com");
        

        var json = await File.ReadAllTextAsync("Features/StripeTests/TestData/payment_intent_succeeded.json");
        var header = StripeTestHelpers.BuildTestHeader(json, stripeConfig.WebhookSecret);
        
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
            options,
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

