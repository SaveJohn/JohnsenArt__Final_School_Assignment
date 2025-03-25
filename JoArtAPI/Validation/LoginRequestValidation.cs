using FluentValidation;
using JohnsenArtAPI.Features.Authentication.Models;

namespace JohnsenArtAPI.Validation;

public class LoginRequestValidation : AbstractValidator<LoginRequest>
{
    public LoginRequestValidation()
    {
        // Member ID
        RuleFor(x => x.Email).NotEmpty()
            .WithMessage("Email feltet kan ikke være tomt.");
            
        
        // Password
        RuleFor(x => x.Password).NotEmpty()
            .WithMessage("Passord feltet kan ikke være tomt.");
    }
}