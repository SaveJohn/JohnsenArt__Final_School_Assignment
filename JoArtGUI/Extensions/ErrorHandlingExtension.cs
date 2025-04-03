using Microsoft.AspNetCore.Diagnostics;

namespace JohnsenArtGUI.Extensions;

public static class ErrorHandlingExtension
{
    
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exceptionHandlerPathFeature =
                    context.Features.Get<IExceptionHandlerPathFeature>();
                if (exceptionHandlerPathFeature?.Error != null)
                {
                    Console.WriteLine($"Blazor Error: {exceptionHandlerPathFeature.Error.Message}");
                }
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("An unhandled error occurred.");
            });
        });
        return app;
    }
    
}