using eSecurity.Client.Common.Models;
using eSystem.Core.Validation;
using FluentValidation;

namespace eSecurity.Client.Validation.Validators;

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