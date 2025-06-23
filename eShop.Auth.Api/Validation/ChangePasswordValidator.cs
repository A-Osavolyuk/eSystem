using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Validation;

public class ChangePasswordValidator : Validator<ChangePasswordRequest>
{
    public ChangePasswordValidator()
    {
        RuleFor(p => p.CurrentPassword)
            .NotEmpty().WithMessage("Field is required.");

        RuleFor(p => p.NewPassword)
            .NotEqual(x => x.CurrentPassword).WithMessage("New password cannot be the same with old one.")
            .NotEmpty().WithMessage("Field is required.")
            .MinimumLength(8).WithMessage("New password must be at least 8 characters long.")
            .MaximumLength(32).WithMessage("New password cannot be longer then 32 characters.")
            .Matches("[A-Z]").WithMessage("New password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("New password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("New password must contain at least one numeric digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("New password must contain at least one special character.");

        RuleFor(p => p.ConfirmNewPassword)
            .NotEmpty().WithMessage("You must confirm your new password.")
            .Equal(x => x.NewPassword).WithMessage("Must be the same with new password.");
    }
}