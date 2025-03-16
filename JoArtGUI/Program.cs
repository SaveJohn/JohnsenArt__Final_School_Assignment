using JohnsenArtGUI.Components;
using Blazored.LocalStorage;

var builder = WebApplication.CreateBuilder(args);
// temporary logging increase
builder.Logging.SetMinimumLevel(LogLevel.Debug);


// Dependency Injections
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddHttpClient();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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