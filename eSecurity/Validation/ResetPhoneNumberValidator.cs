using eSystem.Core.Requests.Auth;
using eSystem.Core.Validation;
using eSystem.Core.Validation.Validators;

namespace eSecurity.Validation;

public class ResetPhoneNumberValidator : Validator<ResetPhoneNumberRequest>
{
    public ResetPhoneNumberValidator()
    {
        RuleFor(x => x.NewPhoneNumber).SetValidator(new PhoneNumberValidator());
    }
}