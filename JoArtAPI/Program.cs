using System.Text.Json;
using Amazon;
using Amazon.S3;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using JoArtClassLib.Configuration.Secrets;
using JoArtDataLayer;
using JoArtDataLayer.Repositories;
using JoArtDataLayer.Repositories.Interfaces;
using JohnsenArtAPI.Configuration;
using JohnsenArtAPI.Extensions;
using JohnsenArtAPI.Features.Authentication;
using JohnsenArtAPI.Features.Authentication.Interfaces;
using JohnsenArtAPI.Features.Contact;
using JohnsenArtAPI.Features.Contact.Interfaces;
using JohnsenArtAPI.Features.Gallery.AdminAccess;
using JohnsenArtAPI.Features.Gallery.AdminAccess.Interfaces;
using JohnsenArtAPI.Features.Gallery.Common;
using JohnsenArtAPI.Features.Gallery.Common.Aws;
using JohnsenArtAPI.Features.Gallery.Common.Aws.Interfaces;
using JohnsenArtAPI.Features.Gallery.Common.Interfaces;
using JohnsenArtAPI.Features.Health;
using JohnsenArtAPI.Features.Payments.Controllers;
using JohnsenArtAPI.Features.Payments.Interfaces;
using JohnsenArtAPI.Features.Payments.Services;
using JohnsenArtAPI.Middleware;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using Stripe;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment.EnvironmentName;
var appName = builder.Environment.ApplicationName;


// DEPENDENCY INJECTIONS -----------------------------------------------------------------------------

// Endpoint related 
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Mapper 
builder.Services.AddAutoMapper(typeof(Program));

// Gallery
builder.Services.AddScoped<IAdminGalleryService, AdminGalleryService>();
builder.Services.AddScoped<IGalleryService, GalleryService>();
builder.Services.AddScoped<IAdminGalleryRepository, AdminGalleryRepository>();
builder.Services.AddScoped<IGalleryRepository, GalleryRepository>();

// Health Checks
builder.Services.AddHealthChecks()
    .AddCheck<APIHealthCheck>("api")
    .AddCheck<DatabaseHealthCheck>("database");

// AWS
builder.Services.AddScoped<IAwsService, AwsService>();
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonSecretsManager>();
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.Configure<AwsS3Config>(builder.Configuration.GetSection("AwsS3Settings"));
builder.Configuration.AddSecretsManager(region: RegionEndpoint.EUNorth1,
    configurator: options =>
    {
        options.SecretFilter = entry => entry.Name.StartsWith($"{env}_{appName}_");
        options.KeyGenerator = (entry, s ) => s.Replace($"{env}_{appName}_", string.Empty)
            .Replace("__", ":");
        
        // The following option is good if changes are made in the secrets value within aws secrets manager,
        // but to save costs, I will not enable it :
        // options.PollingInterval = TimeSpan.FromSeconds(10); 
    });

// Database Context
builder.Services.AddDbContext<JoArtDbContext>(options =>
{
    var conn = builder.Configuration["Database:ConnectionString"];
    if (string.IsNullOrWhiteSpace(conn))
        throw new InvalidOperationException("Database connection string not configured!");

    options.UseMySql(
        conn,
        ServerVersion.AutoDetect(conn)
    );
});

//JWT 
builder.Services.Configure<JwtConfig>(
    builder.Configuration.GetSection("Jwt"));

var jwtConfig = builder.Configuration.GetSection("Jwt")
    .Get<JwtConfig>() ?? throw new NullReferenceException("JWT secret not configured!");

builder.Services.AddSingleton(jwtConfig);
builder.Services.AddJwtAuthentication(jwtConfig);
builder.Services.AddScoped<IAuthService, AuthService>();

// Admin
builder.Services.AddScoped<IAdminUserRepository, AdminUserRepository>();

// Stripe
builder.Services.Configure<StripeConfig>(
    builder.Configuration.GetSection("Stripe"));
builder.Services.AddScoped<IStripeService, StripeService>();

// Strip event for Development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton<StripeEventParser>((payload, sig, secret) =>
    {
        return new Event { Type = "payment_intent.succeeded", /* … */ };
    });
}
// Strip event for Production
else 
{
    builder.Services.AddSingleton<StripeEventParser>((payload, sig, secret) =>
        EventUtility.ConstructEvent(
            payload,
            sig,
            secret,
            throwOnApiVersionMismatch: false
        )
    );
}

// Smtp
builder.Services.Configure<SmtpConfig>(
    builder.Configuration.GetSection("Smtp"));

// MailKit
builder.Services.AddScoped<IEmailService, MailKitEmailService>();
builder.Services.AddScoped<IOrderEmailService, OrderEmailService>();

// Logging
builder.Host.UseSerilog((context, services, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
);

// APP -----------------------------------------------------------------------------------------------
var app = builder.Build();

//API Health Check -> Returning json with results for api and database separate 
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description ?? "",
                error = entry.Value.Exception?.Message
            })
        });

        await context.Response.WriteAsync(result);
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options => { options.RouteTemplate = "/openapi/{documentName}.json"; });
    app.MapScalarApiReference();
    /*  --- Use this instead if you prefer swagger GUI: ---
     * app.UseSwagger();
     * app.UseSwaggerUI();
     */
}

app.UseMiddleware<GlobalExceptionHandling>()
    .UseHttpsRedirection()
    .UseAuthentication()
    .UseAuthorization();

app.MapControllers();

await app.RunAsync();

public partial class Program { } // For integration testing