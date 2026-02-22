using eSecurity.Client.Common.Models;
using eSystem.Core.Validation;
using eSystem.Core.Validation.Validators;
using FluentValidation;

namespace eSecurity.Client.Validation.Validators;

public class ResetPasswordValidator : Validator<ResetPasswordModel>
{
    public ResetPasswordValidator()
    {
        RuleFor(p => p.Password).SetValidator(new PasswordValidator());

        RuleFor(p => p.ConfirmPassword)
            .NotEmpty().WithMessage("You must confirm your new password.")
            .Equal(x => x.Password).WithMessage("Must be the same with new password.");
    }
}