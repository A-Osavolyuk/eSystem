using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Validation;

public class LoginValidator : Validator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(p => p.Login)
            .NotEmpty().WithMessage("Field is required.");

        RuleFor(p => p.Password)
            .NotEmpty().WithMessage("Field is required.");
    }
}