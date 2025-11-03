using eSystem.Core.Requests.Auth;
using eSystem.Core.Validation;
using eSystem.Core.Validation.Validators;

namespace eSecurity.Validation;

public class ChangePhoneNumberValidator : Validator<ChangePhoneNumberRequest>
{
    public ChangePhoneNumberValidator()
    {
        RuleFor(x => x.NewPhoneNumber).SetValidator(new PhoneNumberValidator());
    }
}