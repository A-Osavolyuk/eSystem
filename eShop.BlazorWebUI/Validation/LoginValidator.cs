using eShop.BlazorWebUI.Models;
using eShop.Domain.Abstraction.Validation;
using FluentValidation;

namespace eShop.BlazorWebUI.Validation;

public class LoginValidator : Validator<LoginModel>
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