using eSecurity.Client.Common.Models;
using eSystem.Core.Validation;
using eSystem.Core.Validation.Validators;

namespace eSecurity.Client.Common.Validation.Validators;

public class ChangePhoneNumberValidator : Validator<ChangePhoneNumberModel>
{
    public ChangePhoneNumberValidator()
    {
        RuleFor(x => x.NewPhoneNumber).SetValidator(new PhoneNumberValidator());
    }
}