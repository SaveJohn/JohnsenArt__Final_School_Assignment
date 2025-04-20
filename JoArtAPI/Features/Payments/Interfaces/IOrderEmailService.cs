using JohnsenArtAPI.Features.Contact.DTO;

namespace JohnsenArtAPI.Features.Contact.Interfaces;

public interface IOrderEmailService
{
    Task SendOrderEmailAsync(EmailMessage message);
}