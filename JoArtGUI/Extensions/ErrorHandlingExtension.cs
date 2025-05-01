using Microsoft.AspNetCore.Diagnostics;

namespace JohnsenArtGUI.Extensions;

public static class ErrorHandlingExtension
{
    
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app, ILogger logger)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                if (exceptionHandlerPathFeature?.Error != null)
                {
                    logger.LogError(exceptionHandlerPathFeature.Error, "Unhandled blazor error occurred.");
                }
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("An unhandled error occurred.");
            });
        });
        return app;
    }

}