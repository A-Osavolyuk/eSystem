using eAccount.Blazor.Server.Domain.Models;
using eSystem.Core.Validation;

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