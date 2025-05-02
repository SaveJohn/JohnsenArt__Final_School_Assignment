using System.Linq;
using JoArtClassLib.Art;
using JoArtDataLayer.Repositories.Interfaces;
using JohnsenArtAPI.Features.Gallery.AdminAccess.Interfaces;
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

                // Mock IAdminGalleryService 
                var svcDesc = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IAdminGalleryService));
                if (svcDesc != null) services.Remove(svcDesc);
                services.AddSingleton(AdminGalleryServiceMock.Object);
                
                // Mock IAdminGalleryRepository 
                var repoDesc = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IAdminGalleryRepository));
                if (repoDesc != null) services.Remove(repoDesc);
                services.AddSingleton(AdminGalleryRepositoryMock.Object);
                
            });
        }
    }
}
