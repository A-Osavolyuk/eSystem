using eSecurity.Idp.Common.Validation.Validators.Standard;
using eSecurity.Idp.Features.Password;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Password;

public sealed class ConfirmForgotPasswordCommandValidator : AbstractValidator<ConfirmForgotPasswordCommand>
{
    public ConfirmForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .SetValidator(new EmailValidator("email"));

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("'code' is required");
    }
}