namespace JoArtClassLib.AwsSecrets;

public class JwtSecretConfig
{
    public string Key { get; set; } = "";
    public string Issuer { get; set; } = "";
    public string Audience { get; set; } = "";
}