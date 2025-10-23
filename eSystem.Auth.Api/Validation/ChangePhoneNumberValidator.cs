using eSystem.Application.Validation;
using eSystem.Domain.Requests.Auth;

namespace eSystem.Auth.Api.Validation;

public class ChangePhoneNumberValidator : Validator<ChangePhoneNumberRequest>
{
    public ChangePhoneNumberValidator()
    {
        RuleFor(x => x.NewPhoneNumber).SetValidator(new PhoneNumberValidator());
    }
}