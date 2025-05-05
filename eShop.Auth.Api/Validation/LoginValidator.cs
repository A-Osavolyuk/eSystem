using eShop.Domain.Abstraction.Validation;
using eShop.Domain.Requests.API.Auth;
using FluentValidation;

namespace eShop.Auth.Api.Validation;

public class LoginValidator : Validator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(p => p.Email)
            .NotEmpty().WithMessage("Email is must.")
            .EmailAddress().WithMessage("Invalid format of email address.");

        RuleFor(p => p.Password)
            .NotEmpty().WithMessage("Password is must.");
    }
}