using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Validation;

public class ChangePasswordValidator : Validator<ChangePasswordRequest>
{
    public ChangePasswordValidator()
    {
        RuleFor(p => p.OldPassword)
            .NotEmpty().WithMessage("You must enter your old password.");

        RuleFor(p => p.NewPassword)
            .NotEqual(x => x.OldPassword).WithMessage("New Password cannot be the same with old one.")
            .NotEmpty().WithMessage("New Password is must.")
            .MinimumLength(8).WithMessage("New Password must be at least 8 characters long.")
            .MaximumLength(32).WithMessage("New Password cannot be longer then 32 characters.")
            .Matches("[A-Z]").WithMessage("New Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("New Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("New Password must contain at least one numeric digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("New Password must contain at least one special character.");

        RuleFor(p => p.ConfirmNewPassword)
            .NotEmpty().WithMessage("You must confirm your new password.")
            .Equal(x => x.NewPassword).WithMessage("Must be the same with new password.");
    }
}