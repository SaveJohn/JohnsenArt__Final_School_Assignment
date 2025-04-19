using JohnsenArtAPI.Features.Contact.DTO;

namespace JohnsenArtAPI.Features.Contact.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(EmailRequest emailRequest);
}