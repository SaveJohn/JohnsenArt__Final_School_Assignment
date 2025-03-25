using Amazon.S3;
using AutoMapper;
using JoArtDataLayer;
using JoArtDataLayer.Health;
using JoArtDataLayer.Repositories;
using JoArtDataLayer.Repositories.Interfaces;
using JohnsenArtAPI.Configuration;
using JohnsenArtAPI.Extensions;
using JohnsenArtAPI.Features.Authentication.Interfaces;
using JohnsenArtAPI.Features.Authentication.Services;
using JohnsenArtAPI.Features.Gallery.Admin;
using JohnsenArtAPI.Features.Gallery.Aws;
using JohnsenArtAPI.Features.Gallery.Aws.Interfaces;
using JohnsenArtAPI.Features.Gallery.Common;
using JohnsenArtAPI.Features.Gallery.Common.Interfaces;
using JohnsenArtAPI.Health;
using JohnsenArtAPI.Middleware;
using JohnsenArtAPI.Services;
using JohnsenArtAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Dependency Injection

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Service injections
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminGalleryService, AdminGalleryService>();
builder.Services.AddScoped<IGalleryService, GalleryService>();
builder.Services.AddScoped<IAwsService, AwsService>();

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
    .AddCheck<APIHealthCheck>("api");
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database");


// Database context
builder.Services.AddDbContext<JoArtDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));


//JWT Set up
builder.Services.AddJtwAuthentication(builder.Configuration);


// Logging
builder.Host.UseSerilog((context, services, configuration) => 
    configuration.ReadFrom.Configuration(context.Configuration)
);

var app = builder.Build();

//API Health Check
app.MapHealthChecks("/health");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>    {
        options.RouteTemplate = "/openapi/{documentName}.json";
    });
    app.MapScalarApiReference();
    /*
     * app.UseSwagger();
     * app.UseSwaggerUI();
     */
    
}

app.UseMiddleware<GlobalExceptionHandling>()
    .UseHttpsRedirection()
    .UseHealthChecks("/health")
    .UseAuthentication()
    .UseAuthorization();

app.MapControllers();

await app.RunAsync();