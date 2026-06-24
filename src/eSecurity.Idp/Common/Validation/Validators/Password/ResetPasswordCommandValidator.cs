using eSecurity.Idp.Features.Password;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Password;

public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("'email' is required")
            .EmailAddress().WithMessage("'email' is invalid");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("'password' is required");
    }
}