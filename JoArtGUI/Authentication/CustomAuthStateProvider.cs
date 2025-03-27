using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace JohnsenArtGUI.Authentication;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    
    // Storing current user in memory
    private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
    
    // If this is false, we skip JS calls in GetAuthenticationStateAsync (prerender)
    private bool _initialized;

    public CustomAuthStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    // Getting current user
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Returning anonymous mode if local storage has not been loaded (prerender)
        if (!_initialized)
        {
            return Task.FromResult(new AuthenticationState(_currentUser));
        }

        // Returning the updated user after load from local storage
        return Task.FromResult(new AuthenticationState(_currentUser));
    }

    // Called after prerender (from OnAfterRenderAsync in App.razor)
    public async Task LoadUserFromLocalStorageAsync()
    {
        _initialized = true;
        
        var token = await _localStorage.GetItemAsStringAsync("jwt");
        if (!string.IsNullOrWhiteSpace(token))
        {
            // successful token => logged in user
            var claims = ParseJwt(token);
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
        }
        else
        {
            // No token => remain anonymous
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        }

        // Notifying Blazor that auth state changed
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(_currentUser))
        );
    }

    
    public async Task MarkUserAsLoggedInAsync(string token)
    {
        _initialized = true;
        await _localStorage.SetItemAsStringAsync("jwt", token);
        
        var claims = ParseJwt(token);
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(_currentUser))
        );
    }

    
    public async Task MarkUserAsLoggedOutAsync()
    {
        _initialized = true;
        await _localStorage.RemoveItemAsync("jwt");
        
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(_currentUser))
        );
    }

    private IEnumerable<Claim> ParseJwt(string token)
    {
        // Do not need anything more here yet, but might expand to use role-based user claims
        return new List<Claim> { new Claim(ClaimTypes.Name, "AuthenticatedUser") };
    }
}
