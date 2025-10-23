using eSystem.Application.Validation;
using eSystem.Domain.Requests.Auth;

namespace eSystem.Auth.Api.Validation;

public class ChangePasswordValidator : Validator<ChangePasswordRequest>
{
    public ChangePasswordValidator()
    {
        RuleFor(p => p.CurrentPassword)
            .NotEmpty().WithMessage("Field is required.");

        RuleFor(p => p.NewPassword)
            .NotEqual(x => x.CurrentPassword).WithMessage("New password cannot be the same with old one.")
            .SetValidator(new PasswordValidator());

        RuleFor(p => p.ConfirmNewPassword)
            .NotEmpty().WithMessage("You must confirm your new password.")
            .Equal(x => x.NewPassword).WithMessage("Must be the same with new password.");
    }
}