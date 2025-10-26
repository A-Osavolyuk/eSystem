using eAccount.Blazor.Server.Domain.Models;
using eSystem.Core.Validation;
using eSystem.Core.Validation.Validators;

namespace eAccount.Blazor.Server.Application.Validation;

public class AddPasswordValidator : Validator<AddPasswordModel>
{
    public AddPasswordValidator()
    {
        RuleFor(p => p.Password)
            .SetValidator(new PasswordValidator());

        RuleFor(p => p.ConfirmPassword)
            .NotEmpty().WithMessage("You must confirm your password.")
            .Equal(x => x.Password).WithMessage("Must be the same with new password.");
    }
}