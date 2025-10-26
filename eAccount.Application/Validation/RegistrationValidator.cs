using eAccount.Domain.Models;
using eSystem.Core.Validation;
using eSystem.Core.Validation.Validators;

namespace eAccount.Application.Validation;

public class RegistrationValidator : Validator<RegisterModel>
{
    public RegistrationValidator()
    {
        RuleFor(p => p.Email)
            .NotEmpty().WithMessage("Field is required.")
            .EmailAddress().WithMessage("Invalid format of email address.");
        
        RuleFor(p => p.Username)
            .NotEmpty().WithMessage("Field is required.")
            .MinimumLength(3).WithMessage("Field must be at least 3 characters long.")
            .MaximumLength(32).WithMessage("Field cannot exceed 32 characters.");

        RuleFor(p => p.Password).SetValidator(new PasswordValidator());

        RuleFor(p => p.ConfirmPassword)
            .NotEmpty().WithMessage("You must confirm your password.")
            .Equal(x => x.Password).WithMessage("Must be the same with password.");

    }
}