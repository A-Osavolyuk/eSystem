using eShop.Domain.Requests.API.Auth;

namespace eShop.Application.Validation.Auth;

public class ResetPasswordValidator : Validator<ConfirmResetPasswordRequest>
{
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<ConfirmResetPasswordRequest>
            .CreateWithOptions((ConfirmResetPasswordRequest)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return [];
        return result.Errors.Select(e => e.ErrorMessage);
    };
    
    public ResetPasswordValidator()
    {
        RuleFor(p => p.NewPassword)
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