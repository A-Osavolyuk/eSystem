using eSecurity.Client.Common.Models;
using eSystem.Core.Validation;
using FluentValidation;

namespace eSecurity.Client.Common.Validation.Validators;

public class ForgotPasswordValidator : Validator<ForgotPasswordModel>
{
    public ForgotPasswordValidator()
    {
        RuleFor(p => p.Email)
            .NotEmpty().WithMessage("Email is must.")
            .EmailAddress().WithMessage("Invalid format of email address.");
    }
}