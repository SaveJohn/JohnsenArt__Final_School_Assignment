
using JohnsenArtAPI.Features.Contact.DTO;
using JohnsenArtAPI.Features.Contact.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;

namespace JohnsenArtAPI.Features.Contact.Services;

public class MailKitEmailService : IEmailService
{
    private readonly IConfiguration _config;

    public MailKitEmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(EmailRequest emailRequest)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_config["Smtp:From"]));
        email.To.Add(MailboxAddress.Parse("sebastian.kroger.holmen97@gmail.com"));
        email.Subject = $"Email sendt fra {emailRequest.Name}";

        email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = $@"
        <p><strong>Fra:</strong> {emailRequest.FromEmail}</p>
        <p><strong>Melding:</strong></p>
        <p>{emailRequest.Message.Replace("\n", "<br/>")}</p>"
        };


        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_config["Smtp:Host"], int.Parse(_config["Smtp:Port"]),
            MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_config["Smtp:Username"], _config["Smtp:Password"]);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}