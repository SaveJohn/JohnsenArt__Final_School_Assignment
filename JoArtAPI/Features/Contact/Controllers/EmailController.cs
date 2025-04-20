using JohnsenArtAPI.Features.Contact.DTO;
using JohnsenArtAPI.Features.Contact.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JohnsenArtAPI.Features.Contact.Controllers;


[ApiController]
[Route("api/email")]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmail([FromBody] EmailRequest emailRequest)
    {
        await _emailService.SendContactEmailAsync(emailRequest);
        return Ok(new { Message = "Email ble sendt." });
    }
}