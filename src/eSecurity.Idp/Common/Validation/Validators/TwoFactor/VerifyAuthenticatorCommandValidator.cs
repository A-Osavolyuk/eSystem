using eSecurity.Idp.Features.TwoFactor;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.TwoFactor;

public sealed class VerifyAuthenticatorCommandValidator : AbstractValidator<VerifyAuthenticatorCommand>
{
    public VerifyAuthenticatorCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("'code' is required");

        RuleFor(x => x.Secret)
            .NotEmpty().WithMessage("'secret' is required");
    }
}