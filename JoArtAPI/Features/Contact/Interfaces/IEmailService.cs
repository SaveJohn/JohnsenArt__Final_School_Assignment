using JohnsenArtAPI.Features.Contact.DTO;

namespace JohnsenArtAPI.Features.Contact.Interfaces;

public interface IEmailService
{
    Task SendContactEmailAsync(EmailRequest emailRequest);
    Task<string> GetAdminEmailAsync();
}