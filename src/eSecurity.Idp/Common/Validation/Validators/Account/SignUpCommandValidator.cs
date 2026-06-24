using eSecurity.Idp.Features.Account;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Account;

public sealed class SignUpCommandValidator : AbstractValidator<SignUpCommand>
{
    public SignUpCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("'email' is required")
            .EmailAddress().WithMessage("'email' is invalid");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("'password' is required");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("'username' is required");
    }
}