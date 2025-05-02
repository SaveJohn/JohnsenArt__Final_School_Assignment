using System.Text;
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
using Xunit;

namespace IntegrationTests.Features.StripeTests.UnitTests;

//  a quick test to make sure the webhook fails if stripe doesnt send signature header,
// we expect a BadRequest (400), and the parser shouldn't  be used.

public class StripeWebhook_MissingSignature
{
    [Fact]
    public async Task Handle_MissingStripeSignatureHeader_ReturnsBadRequest()
    {
        // arrange
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


        StripeEventParser fakeParser = (_, _, _) => throw new Exception("Should not be called");

        var controller = new StripeWebhookController(
            mockLogger.Object,
            options,
            mockAdminGallery.Object,
            mockGalleryService.Object,
            mockEmailService.Object,
            mockOrderEmail.Object
        );


        var context = new DefaultHttpContext();
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("{}"));


        controller.ControllerContext = new ControllerContext
        {
            HttpContext = context
        };

        //Act
        var result = await controller.Handle();

        //Assert
        Assert.IsType<BadRequestResult>(result);
    }
}