using eShop.Application.Validation;
using eShop.BlazorWebUI.Models;

namespace eShop.BlazorWebUI.Validation;

public class ResetPasswordValidator : Validator<ResetPasswordModel>
{
    public ResetPasswordValidator()
    {
        RuleFor(p => p.NewPassword).SetValidator(new PasswordValidator());

        RuleFor(p => p.ConfirmNewPassword)
            .NotEmpty().WithMessage("You must confirm your new password.")
            .Equal(x => x.NewPassword).WithMessage("Must be the same with new password.");
    }
}