using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using JohnsenArtAPI.Configuration;
using JohnsenArtAPI.Features.Gallery.Aws.Interfaces;
using JohnsenArtAPI.Services;
using Microsoft.Extensions.Options;

namespace JohnsenArtAPI.Features.Gallery.Aws;

public class AwsService : IAwsService
{
    private readonly IAmazonS3 _s3Client;
    private readonly IOptions<AwsS3Settings> _config;
    private readonly ILogger<AdminGalleryService> _logger;
    private readonly int _expirationInSeconds = 0;
    private readonly string _bucketName;

    public AwsService(
        IAmazonS3 s3Client, 
        IOptions<AwsS3Settings> config, 
        ILogger<AdminGalleryService> logger)
    {
        _s3Client = s3Client;
        _logger = logger;
        _expirationInSeconds = config.Value.FileExpireInSeconds;
        _bucketName = config.Value.BucketName;
    }
    
    // S3 Bucket exists
    public async Task<bool> CheckIfS3BucketExists()
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
    public async Task<string> UploadImageToS3(IFormFile imageFile)
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
    public string GeneratePresignedUrl(string objectKey)
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