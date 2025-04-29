using System.Net.Http.Headers;
using System.Net.Http.Json;
using JohnsenArtAPI.Features.Authentication.Models;
using Xunit;

namespace IntegrationTests.Features.ArtworkTests;

public class UpdateArtworkIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public UpdateArtworkIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
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
    public async Task UpdateArtwork_WithValidInput_ShouldReturnSuccessWithArtwork()
    {
    }

}