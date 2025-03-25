using FluentValidation;
using JoArtClassLib.Art;

namespace JohnsenArtAPI.Validation;

public class ImageRequestValidation : AbstractValidator<ImageRequest>
{
    private static readonly HashSet<string> AllowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/png",
        "image/jpeg",
        "image/gif",
        "image/bmp",
        "image/tiff",
    };
    public ImageRequestValidation()
    {
        // Image-> not null
        RuleFor(x => x.ImageFile)
            .NotNull()
            .WithMessage("Bilde er påkrevd.");
        // Image-> correct file type
        RuleFor(x => x.ImageFile.ContentType)
            .Must(contentType => AllowedMimeTypes.Contains(contentType))
            .WithMessage("Filen må være av gyldig bildetype, eksempel: png, jpg, jpeg, tiff, gif eller bmp");
    }
}