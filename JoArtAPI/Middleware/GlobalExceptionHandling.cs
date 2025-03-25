using Microsoft.AspNetCore.Mvc;

namespace JohnsenArtAPI.Middleware;

public class GlobalExceptionHandling
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandling> _logger;
    
    public GlobalExceptionHandling(RequestDelegate next, ILogger<GlobalExceptionHandling> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            // Catching all unhandled exceptions and logging them
            _logger.LogError(ex, 
                "An error occurred while processing a request. Machine: {MachineName}, TraceId: {TraceId}, Exception: {Message}",
                Environment.MachineName, 
                httpContext.TraceIdentifier, 
                ex.Message);

            // Mapping the exception to an HTTP status code and details
            var (statusCode, title, detail) = MapException(ex);
            
            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/json";

            // Create problem details response
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path,
                Extensions = new Dictionary<string, object?>()
                {
                    { "TraceId", httpContext.TraceIdentifier }
                }
            };
            
            await httpContext.Response.WriteAsJsonAsync(problemDetails);
        }
    }

    // Exception mapping to status code, title, and details
    private static (int statusCode, string title, string detail) MapException(Exception exception)
    {
        return exception switch
        {
            ArgumentNullException => 
                (StatusCodes.Status400BadRequest, 
                 "Bad Request", 
                 "One or more required arguments are missing."),
            
            UnauthorizedAccessException => 
                (StatusCodes.Status401Unauthorized, 
                 "Unauthorized", 
                 "You do not have permission to access this resource."),
            
            KeyNotFoundException => 
                (StatusCodes.Status404NotFound, 
                 "Not Found", 
                 "The requested resource could not be found."),
            
            InvalidOperationException => 
                (StatusCodes.Status409Conflict, 
                 "Conflict", 
                 "The request could not be completed due to a conflict with the current state of the resource."),
            
            _ => 
                (StatusCodes.Status500InternalServerError, 
                 "Internal Server Error", 
                 "An unexpected error occurred on the server.")
        };
    }
    
}