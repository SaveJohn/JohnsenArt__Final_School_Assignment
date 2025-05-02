using System.Net;
using IntegrationTests.Features.ArtworkTests.Interfaces;
using JoArtClassLib.Art;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace IntegrationTests.Features.ArtworkTests.IntegrationTest;

public class DeleteArtworkIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly IAuthenticationHandlerTesting _auth;

    public DeleteArtworkIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _auth = new AuthenticationHandlerTesting(_client);
    }
    
    [Fact]
    public async Task DeleteArtwork_WithExistingId_ShouldReturnSuccessWithDeletedArtwork()
    {
        // -- Arrange ---------------------
        //Authorization: 
        await _auth.Authenticate_WithCorrectCredentials();
        
        var artworkId = 66;
        
        
        // Setting up service mock
        _factory.AdminGalleryServiceMock.Setup(s => 
                s.DeleteArtworkAsync(It.IsAny<int>()))
            .ReturnsAsync((int Id) => new ArtworkResponse
            {
                Id               = artworkId,
                Title            = "Title",
                Description      = "Description",
                Materials        = "Materials",
                ForSale          = true,
                Price            = 100,
                WidthDimension   = 12,
                HeightDimension  = 34,
                HomePageRotation = false,
                Images           = new ()
                
            });

        // -- Act --------------------- 
        
        var endpoint = $"admin/api/Gallery/delete-artwork/{artworkId}";
        var response = await _client.DeleteAsync(endpoint);
        
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var err = await response.Content.ReadAsStringAsync();
            throw new Exception($"Update threw {response.StatusCode}: {err}");
        }
    
        // -- Assert ---------------------
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var artworkResponse = JsonConvert.DeserializeObject<ArtworkResponse>(responseContent);
        
        Assert.NotNull(artworkResponse);
        Assert.IsType<ArtworkResponse>(artworkResponse);
       
        // Artwork 
        Assert.Equal(artworkId, artworkResponse.Id);
        
    }
    
    [Fact]
    public async Task DeleteArtwork_WhenNotAuthenticated_ShouldReturnUnauthorized()
    {
        // -- Arrange ---------------------
        //Authorization: 
        await _auth.Authenticate_WithIncorrectCredentials();
        
        var artworkId = 66;
        
        
        // Setting up service mock
        _factory.AdminGalleryServiceMock.Setup(s => 
                s.DeleteArtworkAsync(It.IsAny<int>()))
            .ReturnsAsync((int Id) => new ArtworkResponse
            {
                Id               = artworkId,
                Title            = "Title",
                Description      = "Description",
                Materials        = "Materials",
                ForSale          = true,
                Price            = 100,
                WidthDimension   = 12,
                HeightDimension  = 34,
                HomePageRotation = false,
                Images           = new ()
                
            });

        // -- Act --------------------- 
        
        var endpoint = $"admin/api/Gallery/delete-artwork/{artworkId}";
        var response = await _client.DeleteAsync(endpoint);
        
        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            var err = await response.Content.ReadAsStringAsync();
            throw new Exception($"Update threw {response.StatusCode}: {err}");
        }
    
        // -- Assert ---------------------
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        
    }
}