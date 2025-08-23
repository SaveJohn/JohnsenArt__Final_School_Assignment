namespace JohnsenArtAPI.Features.Biography.Common.Aws.Interfaces;

public interface IBioAwsService
{
    Task<bool> CheckIfS3BucketExists();
    Task<string> UploadFullViewToS3(IFormFile imageFile);
    Task<string> UploadPreviewImageToS3(IFormFile imageFile);
    Task<string> UploadThumbnailToS3(IFormFile imageFile);

    Task<bool> DeleteImageFromS3(string objectKey);
    string GeneratePresignedUrl(string objectKey);
}