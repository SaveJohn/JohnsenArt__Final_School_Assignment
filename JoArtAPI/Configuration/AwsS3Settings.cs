namespace JohnsenArtAPI.Configuration;

public class AwsS3Settings
{
    public int FileExpireInSeconds { get; set; }
    public string BucketName { get; set; }
}