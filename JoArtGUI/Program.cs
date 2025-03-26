using JohnsenArtGUI.Components;
using Blazored.LocalStorage;
using JohnsenArtGUI.Authentication;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


// temporary logging increase
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Dependency Injections
builder.Services.AddHttpClient();

// Authentication and Authorization
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());


// TEMPORARY FIX TO PROBLEM I DO NOT KNOW HOW TO FIX YET
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:8080")
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


app.UseHttpsRedirection();


app.UseStaticFiles();
app.UseAntiforgery();


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


await app.RunAsync();