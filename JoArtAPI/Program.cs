using Amazon.S3;
using AutoMapper;
using JoArtDataLayer;
using JoArtDataLayer.Repositories;
using JoArtDataLayer.Repositories.Interfaces;
using JohnsenArtAPI.Configuration;
using JohnsenArtAPI.Features.Authentication.Interfaces;
using JohnsenArtAPI.Features.Authentication.Services;
using JohnsenArtAPI.Health;
using JohnsenArtAPI.Services;
using JohnsenArtAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Dependency Injection
builder.Services.AddScoped<IAuthService, AuthService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Service injections
builder.Services.AddScoped<IAdminGalleryService, AdminGalleryService>();

// Mapper injections
builder.Services.AddAutoMapper(typeof(Program));

// Repository injections
builder.Services.AddScoped<IAdminGalleryRepository, AdminGalleryRepository>();


// AWS
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.Configure<AwsS3Settings>(builder.Configuration.GetSection("AwsS3Settings"));

// API Health Check
builder.Services.AddHealthChecks()
    .AddCheck<APIHealthCheck>("api");


// Database context
builder.Services.AddDbContext<JoArtDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

// Retrieving and checking whether jwt key is present
var jwtKey64 = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey64))
{
    throw new InvalidOperationException("The JWT key is missing, check user secrets/appsettings.json.");
}

var key = new SymmetricSecurityKey(Convert.FromBase64String(builder.Configuration["Jwt:Key"]!));
//JWT Set up
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true
    };
});
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

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();