using eShop.Application.Validation;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Validation;

public class ResetPasswordValidator : Validator<ConfirmResetPasswordRequest>
{
    public ResetPasswordValidator()
    {
        RuleFor(p => p.NewPassword).SetValidator(new PasswordValidator());

        RuleFor(p => p.ConfirmNewPassword)
            .NotEmpty().WithMessage("You must confirm your new password.")
            .Equal(x => x.NewPassword).WithMessage("Must be the same with new password.");
    }
}