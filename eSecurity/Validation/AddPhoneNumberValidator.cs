using eSystem.Core.Requests.Auth;
using eSystem.Core.Validation;
using eSystem.Core.Validation.Validators;

namespace eSecurity.Validation;

public class AddPhoneNumberValidator : Validator<AddPhoneNumberRequest>
{
    public AddPhoneNumberValidator()
    {
        RuleFor(model => model.PhoneNumber).SetValidator(new PhoneNumberValidator());
    }
}