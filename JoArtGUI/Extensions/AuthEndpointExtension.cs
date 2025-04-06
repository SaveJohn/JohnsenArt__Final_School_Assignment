using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace JohnsenArtGUI.Extensions;

public static class AuthEndpointExtension
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/complete-signin", async (string urlRedirct, string token, HttpContext context) =>
        {
            try
            {
                // Read and validate the token.
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                // Create a ClaimsIdentity using the token's claims.
                var identity = new ClaimsIdentity(jwtToken.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
                
                // Store the token as a claim.
                identity.AddClaim(new Claim("JWT", token));

                var principal = new ClaimsPrincipal(identity);

                // Setting the cookie - signing in the user
                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Redirect to the home page.
                return Results.Redirect(urlRedirct);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        });

        endpoints.MapGet("/api/logout", async (HttpContext context) =>
        {
            // Sign out the user by deleting cookie
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            // Redirect the user to home page after logout.
            return Results.Redirect("/");
        });

        return endpoints;
    }
}