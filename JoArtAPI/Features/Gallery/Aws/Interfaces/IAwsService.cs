namespace JohnsenArtAPI.Features.Gallery.Aws.Interfaces;

public interface IAwsService
{
    public Task<bool> CheckIfS3BucketExists();
    public Task<string> UploadImageToS3(IFormFile imageFile);
    public string GeneratePresignedUrl(string objectKey);

}