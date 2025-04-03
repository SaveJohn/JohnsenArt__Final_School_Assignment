using System.IdentityModel.Tokens.Jwt;
using JohnsenArtGUI.Components;
using Syncfusion.Blazor;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using JoArtClassLib;
using Microsoft.AspNetCore.Authentication;

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NNaF5cXmBCekx1WmFZfVtgcl9DYFZTQmYuP1ZhSXxWdkZhXn9YdXRXQGdcWEV9XUs=");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


// temporary logging increase
builder.Logging.SetMinimumLevel(LogLevel.Debug);


// Dependency Injections
builder.Services.AddHttpClient();
builder.Services.AddSyncfusionBlazor();


// Authentication and Authorization
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login"; 
        options.Cookie.HttpOnly = false; // OBS switch to true in production 
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None; // OBS Switch to always in production
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();
builder.Services.AddScoped<ExternalApiService>();



// TEMPORARY FIX TO PROBLEM I DO NOT KNOW HOW TO FIX YET
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:8080")
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature =
            context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature?.Error != null)
        {
            Console.WriteLine($"Blazor Error: {exceptionHandlerPathFeature.Error.Message}");
        }

        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("An unhandled error occurred.");
    });
});

app.MapGet("/api/complete-signin", async (string token, HttpContext context) =>
{
    try
    {
        // Use JwtSecurityTokenHandler to read the token.
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // (Optionally) validate the token's expiration, issuer, etc.
        // For now, we assume the token is valid if it can be read.

        // Create a ClaimsIdentity from the token's claims.
        var identity = new ClaimsIdentity(jwtToken.Claims, CookieAuthenticationDefaults.AuthenticationScheme);

        // Optionally, store the token as a claim if needed later.
        identity.AddClaim(new Claim("JWT", token));

        var principal = new ClaimsPrincipal(identity);

        // Sign in the user on the Blazor Server app (sets the cookie).
        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        // Redirect the user to the home page.
        return Results.Redirect("/");
    }
    catch (Exception ex)
    {
        // If something goes wrong, return an error (or redirect to an error page).
        return Results.BadRequest(new { error = ex.Message });
    }
});
app.MapGet("/api/logout", async (HttpContext context) =>
{
    // Sign out the user (this modifies headers and sets the cookie to expire)
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    // Redirect the user to the login page after logout
    return Results.Redirect("/");
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


await app.RunAsync();

