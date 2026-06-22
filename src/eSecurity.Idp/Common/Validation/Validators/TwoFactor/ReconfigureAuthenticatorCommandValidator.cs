using eSecurity.Idp.Features.TwoFactor;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.TwoFactor;

public sealed class ReconfigureAuthenticatorCommandValidator : AbstractValidator<ReconfigureAuthenticatorCommand>
{
    public ReconfigureAuthenticatorCommandValidator()
    {
        RuleFor(x => x.Secret)
            .NotEmpty().WithMessage("'secret' is required");
    }
}