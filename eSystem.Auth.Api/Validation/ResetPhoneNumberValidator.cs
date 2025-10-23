using eSystem.Application.Validation;
using eSystem.Domain.Requests.Auth;

namespace eSystem.Auth.Api.Validation;

public class ResetPhoneNumberValidator : Validator<ResetPhoneNumberRequest>
{
    public ResetPhoneNumberValidator()
    {
        RuleFor(x => x.NewPhoneNumber).SetValidator(new PhoneNumberValidator());
    }
}