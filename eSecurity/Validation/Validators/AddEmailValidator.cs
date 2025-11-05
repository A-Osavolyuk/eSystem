using eSecurity.Common.Models;
using eSystem.Core.Validation;

namespace eSecurity.Validation.Validators;

public class AddEmailValidator : Validator<AddEmailModel>
{
    public AddEmailValidator()
    {
        RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid email address");
    }
}