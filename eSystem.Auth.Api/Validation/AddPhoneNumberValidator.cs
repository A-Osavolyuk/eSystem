using eSystem.Application.Validation;
using eSystem.Domain.Requests.Auth;

namespace eSystem.Auth.Api.Validation;

public class AddPhoneNumberValidator : Validator<AddPhoneNumberRequest>
{
    public AddPhoneNumberValidator()
    {
        RuleFor(model => model.PhoneNumber).SetValidator(new PhoneNumberValidator());
    }
}