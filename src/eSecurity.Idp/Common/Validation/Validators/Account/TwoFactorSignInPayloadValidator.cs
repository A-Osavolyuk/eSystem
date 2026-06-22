using eSecurity.Core.Security.Authentication.SignIn;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Account;

public sealed class TwoFactorSignInPayloadValidator : AbstractValidator<TwoFactorSignInPayload>
{
    public TwoFactorSignInPayloadValidator()
    {
        RuleFor(x => x.Payload)
            .NotNull().WithMessage("'payload' is required");
    }
}