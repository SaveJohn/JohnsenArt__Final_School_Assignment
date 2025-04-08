namespace JohnsenArtAPI.Features.Gallery.Aws.Interfaces;

public interface IAwsService
{
    public Task<bool> CheckIfS3BucketExists();
    public Task<string> UploadImageToS3(IFormFile imageFile);
    Task<string> UploadThumbnailToS3(IFormFile imageFile);

    public Task<bool> DeleteImageFromS3(string objectKey);
    public string GeneratePresignedUrl(string objectKey);
}