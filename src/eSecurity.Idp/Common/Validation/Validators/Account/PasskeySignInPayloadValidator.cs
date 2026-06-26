using eSecurity.Core.Security.Authentication.SignIn;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Account;

public sealed class PasskeySignInPayloadValidator : AbstractValidator<SoftwareKeySignInPayload>
{
    public PasskeySignInPayloadValidator()
    {
        RuleFor(x => x.Credential)
            .NotNull().WithMessage("'credential' is required");
    }
}