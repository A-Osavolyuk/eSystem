using eShop.BlazorWebUI.Models;

namespace eShop.BlazorWebUI.Validation;

public class LoginValidator : Validator<LoginModel>
{
    public LoginValidator()
    {
        RuleFor(p => p.Login)
            .NotEmpty().WithMessage("Field is required.");

        RuleFor(p => p.Password)
            .NotEmpty().WithMessage("Field is required.");
    }
}