using JoArtClassLib.Configuration.Secrets;
using JohnsenArtAPI.Features.Contact.DTO;
using JohnsenArtAPI.Features.Contact.Interfaces;
using JohnsenArtAPI.Features.Payments.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace JohnsenArtAPI.Features.Payments.Services;

public class OrderEmailService : IOrderEmailService
{
    private readonly SmtpConfig _config;

    public OrderEmailService(IOptions<SmtpConfig> config)
    {
        _config = config.Value;
    }

    public async Task SendOrderEmailAsync(EmailMessage message)
    {
        var email = new MimeMessage();
        
        email.From.Add(MailboxAddress.Parse(_config.From));
        
        email.To.Add(MailboxAddress.Parse(message.ToEmail));

        if (!string.IsNullOrWhiteSpace(message.ReplyTo))
        {
            email.ReplyTo.Add(MailboxAddress.Parse(message.ReplyTo));
        }
        
        email.Subject = message.Subject;

        email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = message.HtmlBody
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_config.Host, int.Parse(_config.Port), 
            MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_config.Username, _config.Password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}