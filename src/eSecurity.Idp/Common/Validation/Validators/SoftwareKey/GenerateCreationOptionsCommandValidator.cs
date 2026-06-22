using eSecurity.Idp.Features.Passkeys;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.SoftwareKey;

public sealed class GenerateCreationOptionsCommandValidator : AbstractValidator<GenerateCreationOptionsCommand>
{
    public GenerateCreationOptionsCommandValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("'display_name' is required");
    }
}