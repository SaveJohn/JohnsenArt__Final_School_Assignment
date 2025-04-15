using System.Text.Json;
using Amazon.S3;
using JoArtDataLayer;
using JoArtDataLayer.Health;
using JoArtDataLayer.Repositories;
using JoArtDataLayer.Repositories.Interfaces;
using JohnsenArtAPI.Configuration;
using JohnsenArtAPI.Extensions;
using JohnsenArtAPI.Features.Authentication.Interfaces;
using JohnsenArtAPI.Features.Authentication.Services;
using JohnsenArtAPI.Features.Contact.Interfaces;
using JohnsenArtAPI.Features.Contact.Services;
using JohnsenArtAPI.Features.Gallery.Admin;
using JohnsenArtAPI.Features.Gallery.Aws;
using JohnsenArtAPI.Features.Gallery.Aws.Interfaces;
using JohnsenArtAPI.Features.Gallery.Common;
using JohnsenArtAPI.Features.Gallery.Common.Interfaces;
using JohnsenArtAPI.Features.Payments.Interfaces;
using JohnsenArtAPI.Features.Payments.Services;
using JohnsenArtAPI.Health;
using JohnsenArtAPI.Middleware;
using JohnsenArtAPI.Services.Interfaces;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Dependency Injection

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Stripe configuration
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// Service injections
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminGalleryService, AdminGalleryService>();
builder.Services.AddScoped<IGalleryService, GalleryService>();
builder.Services.AddScoped<IAwsService, AwsService>();
builder.Services.AddScoped<IStripeService, StripeService>();
builder.Services.AddScoped<IEmailService, MailKitEmailService>();


// Mapper injections
builder.Services.AddAutoMapper(typeof(Program));

// Repository injections
builder.Services.AddScoped<IAdminGalleryRepository, AdminGalleryRepository>();
builder.Services.AddScoped<IGalleryRepository, GalleryRepository>();

// AWS
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.Configure<AwsS3Settings>(builder.Configuration.GetSection("AwsS3Settings"));

// Health Checks
builder.Services.AddHealthChecks()
    .AddCheck<APIHealthCheck>("api")
    .AddCheck<DatabaseHealthCheck>("database");


// Database context
builder.Services.AddDbContext<JoArtDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));


//JWT Set up
builder.Services.AddJwtAuthentication(builder.Configuration);


// Logging
builder.Host.UseSerilog((context, services, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
);

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
    /*
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