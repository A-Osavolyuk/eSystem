using eAccount.Blazor.Server.Domain.Models;

namespace eAccount.Blazor.Server.Application.Validation;

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