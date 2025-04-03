using System.IdentityModel.Tokens.Jwt;
using JohnsenArtGUI.Components;
using Syncfusion.Blazor;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using JohnsenArtGUI.Extensions;
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

app.UseCustomExceptionHandler();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapAuthEndpoints();


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


await app.RunAsync();

