namespace JohnsenArtAPI.Features.Contact.DTO;

public class EmailMessage
{
    public string ToEmail { get; set; }
    public string ReplyTo { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string HtmlBody { get; set; } = string.Empty;
}