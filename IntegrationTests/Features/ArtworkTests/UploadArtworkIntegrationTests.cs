using System.Text;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using JoArtClassLib.Art;
using JohnsenArtAPI.Features.Authentication.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Xunit;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace IntegrationTests.Features.ArtworkTests;

public class UploadArtworkIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public UploadArtworkIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    
    private async Task AuthenticateAsync()
    {
        LoginRequest loginRequest = new LoginRequest() { Email = "test@mail.com", Password = "TestPassword" };
        
        // Uncomment the following line to test with failed authentication:
        // loginRequest = new LoginRequest() { Email = "test@mail.com", Password = "WrongPassword" };
        
        
        var response = await _client.PostAsJsonAsync("admin/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", authResponse.Token);
    }
    
    [Fact]
    public async Task UploadArtwork_ShouldReturnSuccessWithArtwork()
    {
        // Arrange ---------------------
        //Authorization: 
        await AuthenticateAsync();
        
        // Mocking Image Requests
        var bytes = Encoding.UTF8.GetBytes("This is a dummy image file");
        IFormFile imageFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "imageFile.png");

        ImageRequest imageRequest = new ImageRequest { ImageFile = imageFile };
        
        // Mocking Artwork Requests
        ArtworkRequest artworRequest = 
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
        

        // Act --------------------- 
        
        var endpoint = "admin/api/Gallery/upload-artwork";
        var response = await _client.PostAsync(endpoint, content);
        
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var err = await response.Content.ReadAsStringAsync();
            throw new Exception($"Upload gav {response.StatusCode}: {err}");
        }
    
        // Assert ---------------------
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var artworkResponse = JsonConvert.DeserializeObject<ArtworkResponse>(responseContent);
        
        Assert.NotNull(artworkResponse);
        Assert.IsType<ArtworkResponse>(artworkResponse);
       
        // Artwork 
        Assert.Equal(999, artworkResponse.Id);
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
        var image1 = artworkResponse.Images[0];
        Assert.NotEqual(0, image1.Id);
        Assert.NotNull(image1.ObjectKey);
        Assert.NotNull(image1.PreviewKey);
        Assert.NotNull(image1.ThumbnailKey);
        Assert.NotNull(image1.ImageUrl);
        Assert.NotNull(image1.PreviewUrl);
        Assert.NotNull(image1.ThumbnailUrl);
        
        
    }
    
}