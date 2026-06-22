using eSecurity.Core.Security.Authentication.SignIn;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Account;

public sealed class PasswordSignInPayloadValidator : AbstractValidator<PasswordSignInPayload>
{
    public PasswordSignInPayloadValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("'login' is required");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("'password' is required");
    }
}