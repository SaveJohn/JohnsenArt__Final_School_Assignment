namespace JohnsenArtAPI.Features.Contact.DTO;

public class EmailRequest
{
    public string Name { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}