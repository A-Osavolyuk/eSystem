using eSecurity.Idp.Features.Password;
using FluentValidation;

namespace eSecurity.Idp.Common.Validation.Validators.Password;

public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("'current_password' is required");
        
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("'new_password' is required");
    }
}