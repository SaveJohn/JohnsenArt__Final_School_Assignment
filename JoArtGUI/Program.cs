using JohnsenArtGUI.Components;
using Syncfusion.Blazor;
using Microsoft.AspNetCore.Authentication.Cookies;
using JohnsenArtGUI.Extensions;
using JohnsenArtGUI.Helpers;
using JohnsenArtGUI.Helpers.Interfaces;
using Serilog;


Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(
    "Ngo9BigBOggjHTQxAR8/V1NNaF5cXmBCekx1WmFZfVtgcl9DYFZTQmYuP1ZhSXxWdkZhXn9YdXRXQGdcWEV9XUs=");

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
        // options.SlidingExpiration = true; <- This will be used when refresh token is added for JWT in JoArtAPI (will not be done in project-assignment)
        
        // On redirect to login - return to said url after successful login
        options.Events = new CookieAuthenticationEvents()
        {
            OnRedirectToLogin = context =>
            {
                var returnUrl = context.Request.Path + context.Request.QueryString;
                context.Response.Redirect($"/login?returnUrl={Uri.EscapeDataString(returnUrl)}");
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddScoped<ExternalApiService>();

// Local Storage 
builder.Services.AddScoped<ILocalStorageHelper, LocalStorageHelper>();


var apiBaseUrl = builder.Configuration["API_BASE_URL"] ?? "http://joartapi:8080/api";
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl)
});
builder.WebHost.UseUrls("http://0.0.0.0:80");


// Logging
builder.Host.UseSerilog((context, services, loggerConfiguration) =>
    loggerConfiguration.ReadFrom.Configuration(context.Configuration)
);

var app = builder.Build();


//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapAuthEndpoints();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
app.UseCustomExceptionHandler(logger);


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


await app.RunAsync();