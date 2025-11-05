using eSecurity.Common.Models;
using eSystem.Core.Validation;

namespace eSecurity.Validation.Validators;

public class ChangeEmailValidator : Validator<ChangeEmailModel>
{
    public ChangeEmailValidator()
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty().WithMessage("New email is must!")
            .EmailAddress().WithMessage("Invalid email address format.");
    }
}