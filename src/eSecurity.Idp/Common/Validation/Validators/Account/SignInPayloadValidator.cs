using eSecurity.Core.Security.Authentication.SignIn;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Account;

public sealed class SignInPayloadValidator : AbstractValidator<SignInPayload>
{
    public SignInPayloadValidator()
    {
        RuleFor(x => x)
            .SetInheritanceValidator(cfg =>
            {
                cfg.Add(new PasswordSignInPayloadValidator());
                cfg.Add(new PasskeySignInPayloadValidator());
                cfg.Add(new TwoFactorSignInPayloadValidator());
            });
    }
}