using eSecurity.Common.Models;
using eSystem.Core.Validation;
using eSystem.Core.Validation.Validators;

namespace eSecurity.Validation.Validators;

public class AddPhoneNumberValidator : Validator<AddPhoneNumberModel>
{
    public AddPhoneNumberValidator()
    {
        RuleFor(model => model.PhoneNumber).SetValidator(new PhoneNumberValidator());
    }
}