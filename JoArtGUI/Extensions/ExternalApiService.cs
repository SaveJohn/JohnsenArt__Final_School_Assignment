using System.Net.Http.Headers;
using System.Security.Claims;

public class ExternalApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ExternalApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> GetProtectedDataAsync()
    {
        // Retrieve the JWT stored as a claim.
        var token = _httpContextAccessor.HttpContext.User.FindFirst("JWT")?.Value;
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        
        var response = await _httpClient.GetAsync("http://localhost:8080/some-protected-endpoint");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}