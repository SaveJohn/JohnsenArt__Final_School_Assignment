using FluentValidation;
using JoArtClassLib.Art;

namespace JohnsenArtAPI.Validation;

public class ArtworkRequestValidation : AbstractValidator<ArtworkRequest>
{
    public ArtworkRequestValidation()
    {
        // Title
        RuleFor(x => x.Title).NotEmpty()
            .WithMessage("Kunstverket må ha en tittel.");
        // Images
        RuleFor(x => x.Images).NotEmpty()
            .WithMessage("Kunstverket må ha et blide.");
        // Validate each ImageRequest in the list 
        RuleForEach(x => x.Images)
            .SetValidator(new ImageRequestValidation());
    }
    
}