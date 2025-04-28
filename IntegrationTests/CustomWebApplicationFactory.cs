using System.Linq;
using JoArtClassLib.Art;
using JoArtDataLayer.Repositories.Interfaces;
using JohnsenArtAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public Mock<IAdminGalleryRepository> AdminGalleryRepositoryMock { get; }
            = new Mock<IAdminGalleryRepository>();

        public Mock<IAdminGalleryService> AdminGalleryServiceMock { get; }
            = new Mock<IAdminGalleryService>();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                // --- 1) Fjern og mock IAdminGalleryRepository ---
                var repoDesc = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IAdminGalleryRepository));
                if (repoDesc != null) services.Remove(repoDesc);
                services.AddSingleton(AdminGalleryRepositoryMock.Object);

                // --- 2) Fjern og mock IAdminGalleryService ---
                var svcDesc = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IAdminGalleryService));
                if (svcDesc != null) services.Remove(svcDesc);

                AdminGalleryServiceMock
                    .Setup(s => s.UploadArtworkAsync(It.IsAny<ArtworkRequest>()))
                    .Returns((ArtworkRequest req) => Task.FromResult(new ArtworkResponse
                    {
                        Id               = 999,
                        Title            = req.Title,
                        Description      = req.Description,
                        Materials        = req.Materials,
                        ForSale          = req.ForSale,
                        Price            = req.Price,
                        WidthDimension   = req.WidthDimension,
                        HeightDimension  = req.HeightDimension,
                        HomePageRotation = req.HomePageRotation,
                        Images           = req.Images
                            .Select((imgReq, idx) => new ImageResponse
                            {
                                Id           = idx + 1,
                                ObjectKey    = $"obj-{idx+1}",
                                PreviewKey   = $"prev-{idx+1}",
                                ThumbnailKey = $"thumb-{idx+1}",
                                ImageUrl     = $"https://cdn/test/{imgReq.ImageFile.FileName}",
                                PreviewUrl   = $"https://cdn/test/prev/{imgReq.ImageFile.FileName}",
                                ThumbnailUrl = $"https://cdn/test/thumb/{imgReq.ImageFile.FileName}"
                            })
                            .ToList()
                    }));

                services.AddSingleton(AdminGalleryServiceMock.Object);

                
            });
        }
    }
}
