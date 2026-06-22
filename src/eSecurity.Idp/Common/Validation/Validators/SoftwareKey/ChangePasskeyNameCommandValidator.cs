using eSecurity.Idp.Features.Passkeys;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.SoftwareKey;

public sealed class ChangePasskeyNameCommandValidator : AbstractValidator<ChangePasskeyNameCommand>
{
    public ChangePasskeyNameCommandValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("'display_name' is required");
    }
}