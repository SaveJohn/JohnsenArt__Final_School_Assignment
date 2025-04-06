using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using JohnsenArtAPI.Configuration;
using JohnsenArtAPI.Features.Gallery.Admin;
using JohnsenArtAPI.Features.Gallery.Aws.Interfaces;
using JohnsenArtAPI.Services;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

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

    // CHECK If S3 Bucket Exists
    public async Task<bool> CheckIfS3BucketExists()
    {
        _logger.LogInformation($"-------------------- \n AWS: CheckIfS3BucketExists: {_bucketName}");

        var bucketExist = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, _bucketName);
        if (!bucketExist)
        {
            _logger.LogWarning($"Bucket did not exist. Creating new bucket: {_bucketName}");
            var createBucketRequest = new PutBucketRequest
            {
                BucketName = _bucketName,
                UseClientRegion = true
            };
            await _s3Client.PutBucketAsync(createBucketRequest);
        }

        return bucketExist;
    }

    // UPLOAD Image to S3 Bucket
    public async Task<string> UploadImageToS3(IFormFile imageFile)
    {
        _logger.LogInformation($"-------------------- \n AWS: UploadImageToS3: \n File: {imageFile.FileName}:");

        // Creating Object Key
        var objectKey = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
        _logger.LogInformation($"Generated object key: {objectKey}");

        // Creating S3 ObjectRequest
        var objectRequest = new PutObjectRequest
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

    // DELETE Image From S3
    public async Task<bool> DeleteImageFromS3(string objectKey)
    {
        _logger.LogInformation($"-------------------- \n AWS: DeleteImageFromS3: \n  {objectKey}:");

        try
        {
            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = objectKey
            };
            var deleteObjectResponse = await _s3Client.DeleteObjectAsync(deleteObjectRequest);
            _logger.LogInformation($"Deleted object key: {objectKey}");

            return deleteObjectResponse.HttpStatusCode == HttpStatusCode.OK;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError($"Error encountered on server. Message:'{ex.Message}' when deleting an object.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error encountered on server. Message:{ex.Message}");
            throw;
        }
    }

    // CREATE thumbnail of uploaded image
    public async Task<string> UploadThumbnailToS3(IFormFile imageFile)
    {
        using var image = await Image.LoadAsync(imageFile.OpenReadStream());
        image.Mutate(x => x.Resize(400, 0)); // width 400px, preserve aspect ratio

        using var ms = new MemoryStream();
        await image.SaveAsJpegAsync(ms);
        ms.Position = 0;

        var key = $"thumbs/{Guid.NewGuid()}.jpg";

        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = ms,
            ContentType = "image/jpeg"
        };

        await _s3Client.PutObjectAsync(putRequest);
        return key;
    }


    // GENERATE Image Pre-signed URL
    public string GeneratePresignedUrl(string objectKey)
    {
        if (string.IsNullOrWhiteSpace(objectKey))
        {
            _logger.LogWarning("GeneratePresignedUrl was called with a null or empty objectKey.");
            return null!;
        }

        _logger.LogInformation($"-------------------- \n AWS: GeneratePresignedUrl: \n ObjectKey: {objectKey}:");

        try
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = objectKey,
                Expires = DateTime.UtcNow.AddSeconds(_expirationInSeconds)
            };

            var url = _s3Client.GetPreSignedURL(request);
            _logger.LogInformation($"Generated pre-signed URL: {url}");

            return url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating presigned URL for {ObjectKey}", objectKey);
            return null!;
        }
    }

}