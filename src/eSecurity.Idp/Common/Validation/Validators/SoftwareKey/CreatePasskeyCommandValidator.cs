using eSecurity.Idp.Features.Passkeys;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.SoftwareKey;

public sealed class CreatePasskeyCommandValidator : AbstractValidator<CreatePasskeyCommand>
{
    public CreatePasskeyCommandValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("'display_name' is required");

        RuleFor(x => x.Response)
            .NotNull().WithMessage("'response' is required");
    }
}