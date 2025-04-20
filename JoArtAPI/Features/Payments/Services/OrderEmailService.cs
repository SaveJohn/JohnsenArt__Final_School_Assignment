
using JohnsenArtAPI.Features.Contact.DTO;
using JohnsenArtAPI.Features.Contact.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;

namespace JohnsenArtAPI.Features.Contact.Services;

public class OrderEmailService : IOrderEmailService
{
    private readonly IConfiguration _config;

    public OrderEmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendOrderEmailAsync(EmailMessage message)
    {
        var email = new MimeMessage();
        
        email.From.Add(MailboxAddress.Parse(_config["Smtp:From"]));
        
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
        await smtp.ConnectAsync(_config["Smtp:Host"], int.Parse(_config["Smtp:Port"]), 
            MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_config["Smtp:Username"], _config["Smtp:Password"]);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}