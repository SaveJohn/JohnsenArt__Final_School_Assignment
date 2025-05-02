using System.Net;
using System.Net.Http.Headers;
using System.Text;
using IntegrationTests.Features.ArtworkTests.Interfaces;
using JoArtClassLib.Art;
using JoArtClassLib.Art.Artwork;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace IntegrationTests.Features.ArtworkTests.IntegrationTest;

public class UpdateArtworkIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly IAuthenticationHandlerTesting _auth;
    private readonly HttpClient _client;

    public UpdateArtworkIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _auth    = new AuthenticationHandlerTesting(_client);
    }
    
    [Fact]
    public async Task UpdateArtwork_WithValidInput_ShouldReturnSuccessWithUpdatedArtwork()
    {
        // -- Arrange ---------------------
        //Authorization: 
        await _auth.Authenticate_WithCorrectCredentials();
        
        var artworkId = 66;
        
        // Mocking Image Requests
        var bytes = Encoding.UTF8.GetBytes("This is a dummy image file");
        IFormFile imageFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "imageFile.png");

        UpdateImageRequest imageRequest = new UpdateImageRequest { Id = 99, ImageFile = imageFile, ArtworkId = artworkId };
        
        // Mocking Artwork Requests
        UpdateArtworkRequest artworRequest = 
            new ()
            {
                Title = "Lonely Mountain",
                Artist = "Bilbo Baggins",
                Description = "Far away - in a Lonely Mountain - rests Smaug the greedy dragon. There he is guarding his loot, and a stolen home.",
                Materials = "Ink on canvas",
                ForSale = true,
                Price = 7000,
                WidthDimension = 50f,
                HeightDimension = 90f,
                HomePageRotation = false,
                Images = new ()
                {
                    imageRequest
                }
            };

        var content = new MultipartFormDataContent();
        content.Add(new StringContent(artworRequest.Title), "Title");
        content.Add(new StringContent(artworRequest.Artist), "Artist");
        content.Add(new StringContent(artworRequest.Description ?? ""), "Description");
        content.Add(new StringContent(artworRequest.Materials ?? ""), "Materials");
        content.Add(new StringContent(artworRequest.ForSale.ToString()), "ForSale");
        content.Add(new StringContent(artworRequest.Price?.ToString() ?? ""), "Price");
        content.Add(new StringContent(artworRequest.WidthDimension.ToString() ?? ""), "WidthDimension");
        content.Add(new StringContent(artworRequest.HeightDimension.ToString() ?? ""), "HeightDimension");
        content.Add(new StringContent(artworRequest.HomePageRotation.ToString()), "HomePageRotation");
        
        // Adding List of Images to content (using a helper index "0")
        content.Add(new StringContent("0"), "Images.Index"); 
        
        // Copying to memory stream
        var file = imageRequest.ImageFile;    
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        ms.Position = 0;
        
        // Setting up - and adding StreamContent to content
        var fileContent = new StreamContent(ms);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        
        content.Add(
            fileContent,
            "Images[0].ImageFile",     
            file.FileName              
        );
        
        // Setting up service mock
        _factory.AdminGalleryServiceMock.Setup(s => 
                s.UpdateArtworkAsync(It.IsAny<int>(), It.IsAny<UpdateArtworkRequest>()))
            .ReturnsAsync((int Id, UpdateArtworkRequest request) => new ArtworkResponse
            {
                Id               = artworkId,
                Title            = request.Title,
                Description      = request.Description,
                Materials        = request.Materials,
                ForSale          = request.ForSale,
                Price            = request.Price,
                WidthDimension   = request.WidthDimension,
                HeightDimension  = request.HeightDimension,
                HomePageRotation = request.HomePageRotation,
                Images           = request.Images
                    .Select((imgReq, idx) => new ImageResponse
                    {
                        Id           = artworRequest.Images[0].Id+idx,
                        FullViewKey    = $"new-FullVeiwKey-{idx}",
                        PreviewKey   = $"new-PreviewKey-{idx}",
                        ThumbnailKey = $"new-ThumbnailKey-{idx}",
                        FullViewUrl     = $"https://test/full-{imgReq.ImageFile.FileName}",
                        PreviewUrl   = $"https://test/prev-{imgReq.ImageFile.FileName}",
                        ThumbnailUrl = $"https://test/thumb-{imgReq.ImageFile.FileName}"
                    })
                    .ToList()
            });

        // -- Act --------------------- 
        
        var endpoint = $"admin/api/Gallery/update-artwork/{artworkId}";
        var response = await _client.PutAsync(endpoint, content);
        
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
        Assert.Equal(artworkResponse.Title, artworRequest.Title);
        Assert.Equal(artworkResponse.Description, artworRequest.Description);
        Assert.Equal(artworkResponse.Materials, artworRequest.Materials);
        Assert.Equal(artworkResponse.ForSale, artworRequest.ForSale);
        Assert.Equal(artworkResponse.Price, artworRequest.Price);
        Assert.Equal(artworkResponse.WidthDimension, artworRequest.WidthDimension);
        Assert.Equal(artworkResponse.HeightDimension, artworRequest.HeightDimension);
        Assert.Equal(artworkResponse.HomePageRotation, artworRequest.HomePageRotation);
        // Image 
        Assert.NotNull(artworkResponse.Images[0]);
        var responseImage = artworkResponse.Images[0];
        Assert.NotEqual(0, responseImage.Id);
        Assert.NotNull(responseImage.FullViewKey);
        Assert.NotNull(responseImage.PreviewKey);
        Assert.NotNull(responseImage.ThumbnailKey);
        Assert.NotNull(responseImage.FullViewUrl);
        Assert.NotNull(responseImage.PreviewUrl);
        Assert.NotNull(responseImage.ThumbnailUrl);
        
    }
    
    [Fact]
    public async Task UpdateArtwork_WhenNotAuthenticated_ShouldReturnUnauthorized()
    {
        // -- Arrange ---------------------
        //Authorization: 
        await _auth.Authenticate_WithIncorrectCredentials();
        
        var artworkId = 66;
        
        // Mocking Image Requests
        var bytes = Encoding.UTF8.GetBytes("This is a dummy image file");
        IFormFile imageFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "imageFile.png");

        UpdateImageRequest imageRequest = new UpdateImageRequest { Id = 99, ImageFile = imageFile, ArtworkId = artworkId };
        
        // Mocking Artwork Requests
        UpdateArtworkRequest artworRequest = 
            new ()
            {
                Title = "Lonely Mountain",
                Artist = "Bilbo Baggins",
                Description = "Far away - in a Lonely Mountain - rests Smaug the greedy dragon. There he is guarding his loot, and a stolen home.",
                Materials = "Ink on canvas",
                ForSale = true,
                Price = 7000,
                WidthDimension = 50f,
                HeightDimension = 90f,
                HomePageRotation = false,
                Images = new ()
                {
                    imageRequest
                }
            };

        var content = new MultipartFormDataContent();
        content.Add(new StringContent(artworRequest.Title), "Title");
        content.Add(new StringContent(artworRequest.Artist), "Artist");
        content.Add(new StringContent(artworRequest.Description ?? ""), "Description");
        content.Add(new StringContent(artworRequest.Materials ?? ""), "Materials");
        content.Add(new StringContent(artworRequest.ForSale.ToString()), "ForSale");
        content.Add(new StringContent(artworRequest.Price?.ToString() ?? ""), "Price");
        content.Add(new StringContent(artworRequest.WidthDimension.ToString() ?? ""), "WidthDimension");
        content.Add(new StringContent(artworRequest.HeightDimension.ToString() ?? ""), "HeightDimension");
        content.Add(new StringContent(artworRequest.HomePageRotation.ToString()), "HomePageRotation");
        
        // Adding List of Images to content (using a helper index "0")
        content.Add(new StringContent("0"), "Images.Index"); 
        
        // Copying to memory stream
        var file = imageRequest.ImageFile;    
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        ms.Position = 0;
        
        // Setting up - and adding StreamContent to content
        var fileContent = new StreamContent(ms);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        
        content.Add(
            fileContent,
            "Images[0].ImageFile",     
            file.FileName              
        );
        
        // Setting up service mock
        _factory.AdminGalleryServiceMock.Setup(s => 
                s.UpdateArtworkAsync(It.IsAny<int>(), It.IsAny<UpdateArtworkRequest>()))
            .ReturnsAsync((int Id, UpdateArtworkRequest request) => new ArtworkResponse
            {
                Id               = artworkId,
                Title            = request.Title,
                Description      = request.Description,
                Materials        = request.Materials,
                ForSale          = request.ForSale,
                Price            = request.Price,
                WidthDimension   = request.WidthDimension,
                HeightDimension  = request.HeightDimension,
                HomePageRotation = request.HomePageRotation,
                Images           = request.Images
                    .Select((imgReq, idx) => new ImageResponse
                    {
                        Id           = artworRequest.Images[0].Id+idx,
                        FullViewKey    = $"new-FullVeiwKey-{idx}",
                        PreviewKey   = $"new-PreviewKey-{idx}",
                        ThumbnailKey = $"new-ThumbnailKey-{idx}",
                        FullViewUrl     = $"https://test/full-{imgReq.ImageFile.FileName}",
                        PreviewUrl   = $"https://test/prev-{imgReq.ImageFile.FileName}",
                        ThumbnailUrl = $"https://test/thumb-{imgReq.ImageFile.FileName}"
                    })
                    .ToList()
            });

        // -- Act --------------------- 
        
        var endpoint = $"admin/api/Gallery/update-artwork/{artworkId}";
        var response = await _client.PutAsync(endpoint, content);
        
        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            var err = await response.Content.ReadAsStringAsync();
            throw new Exception($"Update threw {response.StatusCode}: {err}");
        }
    
        // -- Assert ---------------------
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
}