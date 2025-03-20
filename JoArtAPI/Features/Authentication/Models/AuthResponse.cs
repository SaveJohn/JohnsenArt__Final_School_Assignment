namespace JohnsenArtAPI.Features.Authentication.Models;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public bool WasSuccessful => string.IsNullOrEmpty(ErrorMessage);
}