using eSecurity.Common.Models;
using eSystem.Core.Validation;
using eSystem.Core.Validation.Validators;

namespace eSecurity.Validation.Validators;

public class ChangePhoneNumberValidator : Validator<ChangePhoneNumberModel>
{
    public ChangePhoneNumberValidator()
    {
        RuleFor(x => x.NewPhoneNumber).SetValidator(new PhoneNumberValidator());
    }
}