using System.Text;
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
        var mockConfig = new Mock<IStripeConfigProvider>();
        var mockAdminGallery = new Mock<IAdminGalleryService>();
        var mockGalleryService = new Mock<IGalleryService>();
        var mockEmailService = new Mock<IEmailService>();
        var mockOrderEmail = new Mock<IOrderEmailService>();

        mockConfig.Setup(c => c.GetStripeConfigAsync())
            .ReturnsAsync(new StripeSecretConfig { WebhookSecret = "whsec_testsecret" });


        StripeEventParser fakeParser = (_, _, _) => throw new Exception("Should not be called");

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