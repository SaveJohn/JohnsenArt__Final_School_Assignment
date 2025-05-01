using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using JohnsenArtAPI.Features.Payments.Controllers;
using JohnsenArtAPI.Services.Interfaces;
using JohnsenArtAPI.Features.Gallery.Common.Interfaces;
using JohnsenArtAPI.Features.Contact.Interfaces;
using JohnsenArtAPI.Features.Payments.Interfaces;
using JoArtClassLib.Art;
using JoArtClassLib.AwsSecrets;
using JoArtClassLib.Payment;
using Stripe;


// test that simulates stripe sending a successful payment intentwebhook
// we expect the artwork to be marked as sold, and a 200 ok to be returned


public class StripeWebhook_PaymentIntentSuccess
{
    [Fact]
    public async Task Handle_PaymentIntentSucceeded_MarksArtworkAsSold()
    {
        //Arrange
        var mockLogger = new Mock<ILogger<StripeWebhookController>>();
        var mockConfig = new Mock<IStripeConfigProvider>();
        var mockAdminGallery = new Mock<IAdminGalleryService>();
        var mockGalleryService = new Mock<IGalleryService>();
        var mockEmailService = new Mock<IEmailService>();
        var mockOrderEmail = new Mock<IOrderEmailService>();

        int testArtworkId = 1;
        var testArtwork = new ArtworkResponse
        {
            Id = testArtworkId,
            Title = "Test Art",
            ForSale = true,
            Price = 2000
        };

        mockConfig.Setup(c => c.GetStripeConfigAsync()).ReturnsAsync(new StripeSecretConfig
        {
            WebhookSecret = "whsec_testsecret"
        });

        mockGalleryService.Setup(g => g.GetArtworkByIdAsync(testArtworkId)).ReturnsAsync(testArtwork);
        mockAdminGallery.Setup(a => a.MarkAsSoldAsync(testArtworkId)).ReturnsAsync(true);
        mockEmailService.Setup(e => e.GetAdminEmailAsync()).ReturnsAsync("admin@example.com");


        StripeEventParser fakeParser = (json, sig, secret) => new Event
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
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("{}"));
        context.Request.Headers["Stripe-Signature"] = "t=12345,v1=fake";

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = context
        };

        // Act
        var result = await controller.Handle();

        //Assert
        Assert.IsType<OkResult>(result);
        mockAdminGallery.Verify(a => a.MarkAsSoldAsync(testArtworkId), Times.Once);
    }
}