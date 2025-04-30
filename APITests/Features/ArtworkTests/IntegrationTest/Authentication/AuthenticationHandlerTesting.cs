using System.Net.Http.Headers;
using System.Net.Http.Json;
using IntegrationTests.Features.ArtworkTests.Interfaces;
using JohnsenArtAPI.Features.Authentication.Models;

namespace IntegrationTests.Features.ArtworkTests;

public class AuthenticationHandlerTesting : IAuthenticationHandlerTesting
{
    
    private readonly HttpClient _client;
    
    public AuthenticationHandlerTesting(HttpClient client)
    {
        _client = client;
    }
    
    /*
     * Remember to add test user to database
     * - Run TestUser.sql -> root/SqlDumps
     *
     * - Or add manually using this mysql script:
     *  use joartdb;
        insert into admins(email, Name ,hashedpassword)
        values ('test@mail.com', 'Test User','$2y$10$.4l4Sd4Et647qIgEhw6TW.O0Hg1qvrGCT/aWMWAyxa0TaFIvjKT4S');
     */ 
    
    
    
    // Successful Authentication
    public async Task Authenticate_WithCorrectCredentials()
    {
        LoginRequest loginRequest = new LoginRequest() { Email = "test@mail.com", Password = "TestPassword" };
        
        var response = await _client.PostAsJsonAsync("admin/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", authResponse.Token);
    }
    
    // Unsuccessful Authentication
    public async Task Authenticate_WithIncorrectCredentials()
    {
        LoginRequest loginRequest = new LoginRequest() { Email = "test@mail.com", Password = "WrongPassword" };
        
        var response = await _client.PostAsJsonAsync("admin/auth/login", loginRequest);
        if (response.IsSuccessStatusCode)
        {
            var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", auth.Token);
        }
        else
        {
            _client.DefaultRequestHeaders.Authorization = null;
        }

    }
}