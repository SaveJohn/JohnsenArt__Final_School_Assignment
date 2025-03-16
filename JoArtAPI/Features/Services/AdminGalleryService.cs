using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using JohnsenArtAPI.Configuration;
using JohnsenArtAPI.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace JohnsenArtAPI.Services;

public class AdminGalleryService : IAdminGalleryService
{
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<AdminGalleryService> _logger;
    private readonly int _expirationInSeconds = 0;
    private readonly string _bucketName;

    public AdminGalleryService(
        IAmazonS3 s3Client, 
        IOptions<AwsS3Settings> config, 
        ILogger<AdminGalleryService> logger)
    {
        _s3Client = s3Client;
        _logger = logger;
        _expirationInSeconds = config.Value.FileExpireInSeconds;
        _bucketName = config.Value.BucketName;
    }
    public async Task<HttpStatusCode> UploadArtworkAsync(IFormFile imageFile)
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
    
        var objectRequest = new PutObjectRequest()
        {
            BucketName = _bucketName, // Using the injected value
            Key = imageFile.FileName,
            InputStream = imageFile.OpenReadStream(),
            StorageClass = S3StorageClass.Standard
        };
    
        var response = await _s3Client.PutObjectAsync(objectRequest);
        return response.HttpStatusCode;
    }

    
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
            return _s3Client.GetPreSignedURL(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating presigned URL for {ObjectKey}", objectKey);
            return null;
        }
    }

}