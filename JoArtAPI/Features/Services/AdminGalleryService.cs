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
using JohnsenArtAPI.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace JohnsenArtAPI.Services;

public class AdminGalleryService : IAdminGalleryService
{
    private readonly IAdminGalleryRepository _repository;
    private readonly IMapper _mapper;
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<AdminGalleryService> _logger;
    private readonly int _expirationInSeconds = 0;
    private readonly string _bucketName;

    public AdminGalleryService(
        IAdminGalleryRepository repository,
        IMapper mapper,
        IAmazonS3 s3Client, 
        IOptions<AwsS3Settings> config, 
        ILogger<AdminGalleryService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _s3Client = s3Client;
        _logger = logger;
        _expirationInSeconds = config.Value.FileExpireInSeconds;
        _bucketName = config.Value.BucketName;
    }
    
    // UPLOAD Artwork
    public async Task<ArtworkResponse> UploadArtworkAsync(ArtworkRequest request)
    {

        _logger.LogDebug($"Number of images in the request: {request.Images?.Count}");



        // Making sure bucket exists (It declared in appsettings.json)
        await CheckIfS3BucketExists();

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
                var objectKey = await UploadImageToS3(image.ImageFile);

                // Adding ObjectKey property value to Artwork entity
                artwork.Images.Add(new ArtworkImage { ObjectKey = objectKey, IsWallPreview = image.IsWallPreview });
            }
        }

        foreach (var image in artwork.Images)
        {
            _logger.LogDebug($"Uploading image {image.ObjectKey}");
        }


        // Saving to database through repository
        var savedArtwork = await _repository.AddArtworkAsync(artwork);

        // Return DTO
        return _mapper.Map<ArtworkResponse>(savedArtwork);
    }

   
    
    // AWS -------------------------------------------------------------------------
    
    // S3 Bucket exists
    private async Task<bool> CheckIfS3BucketExists()
    {
        var bucketExist = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, _bucketName);
        if (!bucketExist)
        {
            var createBucketRequest = new PutBucketRequest()
            {
                BucketName = _bucketName,
                UseClientRegion = true
            };
            await _s3Client.PutBucketAsync(createBucketRequest);
        }
        return bucketExist;
    }
    
    // Uploading ObjectRequest (image) to S3 Bucket
    private async Task<string> UploadImageToS3(IFormFile imageFile)
    {
        _logger.LogInformation($"Uploading {imageFile.FileName} to S3...");

        // Creating S3 ObjectRequest
        var objectKey = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
        _logger.LogInformation($"Generated object key: {objectKey}");

        var objectRequest = new PutObjectRequest()
        {
            BucketName = _bucketName, // Using the injected value
            Key = objectKey,
            InputStream = imageFile.OpenReadStream(),
            StorageClass = S3StorageClass.Standard
        };

        try // Uploading image to S3 bucket
        {
            await _s3Client.PutObjectAsync(objectRequest);
            _logger.LogInformation($"Uploading image {objectKey} to {_bucketName}");
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError($"Uploading image {objectKey} to {_bucketName} failed: {ex.Message}");
            throw;
        }
        return objectKey;
    }
    
    // Generate Image Pre-signed URL
    private string GeneratePresignedUrl(string objectKey)
    {
        try
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName, // Use the configured bucket name
                Key = objectKey,
                Expires = DateTime.UtcNow.AddSeconds(_expirationInSeconds)
            };
            _logger.LogInformation($"Getting pre-signed url for {objectKey}: {request}");
            return _s3Client.GetPreSignedURL(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating presigned URL for {ObjectKey}", objectKey);
            return null;
        }
    }

    
}