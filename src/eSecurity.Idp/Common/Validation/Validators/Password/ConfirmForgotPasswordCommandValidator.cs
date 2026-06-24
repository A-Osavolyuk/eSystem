using eSecurity.Idp.Features.Password;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Password;

public sealed class ConfirmForgotPasswordCommandValidator : AbstractValidator<ConfirmForgotPasswordCommand>
{
    public ConfirmForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("'email' is required")
            .EmailAddress().WithMessage("'email' is invalid");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("'code' is required");
    }
}