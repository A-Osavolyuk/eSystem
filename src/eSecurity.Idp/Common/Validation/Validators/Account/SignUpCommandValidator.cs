using eSecurity.Idp.Common.Validation.Validators.Standard;
using eSecurity.Idp.Features.Account;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Account;

public sealed class SignUpCommandValidator : AbstractValidator<SignUpCommand>
{
    public SignUpCommandValidator()
    {
        RuleFor(x => x.Email)
            .SetValidator(new EmailValidator("email"));

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("'password' is required");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("'username' is required");
    }
}