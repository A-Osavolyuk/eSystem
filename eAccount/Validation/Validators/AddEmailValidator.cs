using eAccount.Common.Models;
using eSystem.Core.Validation;

namespace eAccount.Validation.Validators;

public class AddEmailValidator : Validator<AddEmailModel>
{
    public AddEmailValidator()
    {
        RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid email address");
    }
}