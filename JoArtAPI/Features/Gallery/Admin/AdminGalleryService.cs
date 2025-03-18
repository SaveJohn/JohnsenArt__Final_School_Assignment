using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using JoArtClassLib;
using JoArtClassLib.Art;
using JoArtDataLayer.Repositories.Interfaces;
using JohnsenArtAPI.Configuration;
using JohnsenArtAPI.Features.Gallery.Aws.Interfaces;
using JohnsenArtAPI.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace JohnsenArtAPI.Services;

public class AdminGalleryService : IAdminGalleryService
{
    private readonly IAdminGalleryRepository _repository;
    private readonly IAwsService _aws;
    private readonly IMapper _mapper;
    private readonly ILogger<AdminGalleryService> _logger;

    public AdminGalleryService(
        IAdminGalleryRepository repository,
        IAwsService aws,
        IMapper mapper,
        ILogger<AdminGalleryService> logger)
    {
        _repository = repository;
        _aws = aws;
        _mapper = mapper;
        _logger = logger;
    }
    
    // UPLOAD Artwork
    public async Task<ArtworkResponse> UploadArtworkAsync(ArtworkRequest request)
    {
        _logger.LogInformation($"-------------------- \n Service: UploadArtwork:");
        _logger.LogDebug($"Number of images in the request: {request.Images?.Count}");
        

        // Making sure bucket exists (It declared in appsettings.json)
        await _aws.CheckIfS3BucketExists();

        // Map DTO to Entity
        var artwork = _mapper.Map<Artwork>(request);
        artwork.Images.Clear(); // Remove Automapper placeholders

        // Uploading image(s) to S3 bucket
        foreach (var image in request.Images)
        {
            _logger.LogDebug("Entering image upload loop.");
            _logger.LogDebug($"Processing image: {image.ImageFile?.FileName ?? "No file"}");

            if (image.ImageFile != null)
            {
                var objectKey = await _aws.UploadImageToS3(image.ImageFile);

                // Adding ObjectKey property value to Artwork entity
                artwork.Images.Add(new ArtworkImage { ObjectKey = objectKey, IsWallPreview = image.IsWallPreview });
            }
            else
            {
                _logger.LogWarning($"No image found in image upload loop.");
                throw new Exception("No image found in image upload loop.");
            }
        }
        
        // Saving to database through repository
        var savedArtwork = await _repository.AddArtworkAsync(artwork);

        // Return DTO
        return _mapper.Map<ArtworkResponse>(savedArtwork);
    }

}