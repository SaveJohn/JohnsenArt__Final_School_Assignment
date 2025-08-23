namespace JohnsenArtAPI.Features.Gallery.Common.Aws.Interfaces;

public interface IAwsService
{
    Task<bool> CheckIfS3BucketExists();
    Task<string> UploadFullViewToS3(IFormFile imageFile);
    Task<string> UploadPreviewImageToS3(IFormFile imageFile);
    Task<string> UploadThumbnailToS3(IFormFile imageFile);

    Task<bool> DeleteImageFromS3(string objectKey);
    string GeneratePresignedUrl(string objectKey);
}