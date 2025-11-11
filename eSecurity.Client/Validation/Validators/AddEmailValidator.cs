using eSecurity.Client.Common.Models;
using eSystem.Core.Validation;
using FluentValidation;

namespace eSecurity.Client.Validation.Validators;

public class AddEmailValidator : Validator<AddEmailModel>
{
    public AddEmailValidator()
    {
        RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid email address");
    }
}