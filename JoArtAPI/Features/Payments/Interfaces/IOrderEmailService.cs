using JohnsenArtAPI.Features.Contact.DTO;

namespace JohnsenArtAPI.Features.Payments.Interfaces;

public interface IOrderEmailService
{
    Task SendOrderEmailAsync(EmailMessage message);
}