using eSecurity.Idp.Common.Validation.Validators.Standard;
using eSecurity.Idp.Features.Password;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Password;

public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .SetValidator(new EmailValidator("email"));

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("'password' is required");
    }
}