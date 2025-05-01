
using JoArtDataLayer.Repositories;
using JoArtDataLayer.Repositories.Interfaces;
using JohnsenArtAPI.Features.Contact.DTO;
using JohnsenArtAPI.Features.Contact.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http.HttpResults;
using MimeKit;

namespace JohnsenArtAPI.Features.Contact.Services;

public class MailKitEmailService : IEmailService
{
    private readonly ILogger<MailKitEmailService> _logger;
    private readonly IConfiguration _config;
    private readonly IAdminUserRepository _repository;

    public MailKitEmailService(
        ILogger<MailKitEmailService> logger,
        IConfiguration config,
        IAdminUserRepository repository)
    {
        _logger = logger;
        _config = config;
        _repository = repository;
    }

    public async Task SendContactEmailAsync(EmailRequest emailRequest)
    {
        var email = new MimeMessage();
        var adminEmail = await _repository.GetAdminEmail();
        email.From.Add(MailboxAddress.Parse(_config["Smtp:From"]));
        email.To.Add(MailboxAddress.Parse(adminEmail));
        email.Subject = $"{emailRequest.Name} har sendt deg en mail via JohnsenArt";
        email.ReplyTo.Add(MailboxAddress.Parse(emailRequest.FromEmail));
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = $@"
        <p><strong>Fra:</strong> {emailRequest.FromEmail}</p>
        <p><strong>Melding:</strong></p>
        <p>{emailRequest.Message.Replace("\n", "<br/>")}</p>"
        };


        using var smtp = new SmtpClient();
        
        // Development test-mode certificate (TODO get real certificate for a "noreply@johnsen.art" or similar)
        smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

        await smtp.ConnectAsync(_config["Smtp:Host"], int.Parse(_config["Smtp:Port"]),
            MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_config["Smtp:Username"], _config["Smtp:Password"]);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }

    public async Task<string> GetAdminEmailAsync()
    {
        var email = await _repository.GetAdminEmail();
        return email;
    }
}