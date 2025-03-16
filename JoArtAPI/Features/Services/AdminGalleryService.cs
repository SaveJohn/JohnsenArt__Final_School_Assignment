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

    public AdminGalleryService(
        IAmazonS3 s3Client, 
        IOptions<AwsS3Settings> config, 
        ILogger<AdminGalleryService> logger)
    {
        _s3Client = s3Client;
        _logger = logger;
        _expirationInSeconds = config.Value.FileExpireInSeconds;
    }
    public async Task<HttpStatusCode> UploadArtworkAsync(IFormFile imageFile, string bucketName)
    {
        var bucketExist = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
        if (!bucketExist)
        {
            var createBucketRequest = new PutBucketRequest()
            {
                BucketName = bucketName,
                UseClientRegion = true
            };
            await _s3Client.PutBucketAsync(createBucketRequest);

        }
        
        //upload file to s3
        var objectRequest = new PutObjectRequest()
        {
            BucketName = bucketName,
            Key = imageFile.FileName,
            InputStream = imageFile.OpenReadStream(),
            StorageClass = S3StorageClass.Standard
        };
        
        var response = await _s3Client.PutObjectAsync(objectRequest);
        return response.HttpStatusCode;
    }
    
    private string GeneratePresignedUrl(string bucketName, string objectKey)
    {
        try
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = objectKey,
                Expires = DateTime.UtcNow.AddSeconds(_expirationInSeconds)
            };
            string url = _s3Client.GetPreSignedURL(request);
            return url;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}