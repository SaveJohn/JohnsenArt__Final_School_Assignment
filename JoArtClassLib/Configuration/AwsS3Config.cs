namespace JohnsenArtAPI.Configuration;

public class AwsS3Config
{
    public int FileExpireInSeconds { get; set; }
    public string BucketName { get; set; }
}