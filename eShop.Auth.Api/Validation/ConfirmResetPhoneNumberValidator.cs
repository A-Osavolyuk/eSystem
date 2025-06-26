using eShop.Application.Validation;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Validation;

public class ConfirmResetPhoneNumberValidator : Validator<ConfirmResetPhoneNumberRequest>
{
    public ConfirmResetPhoneNumberValidator()
    {
        RuleFor(x => x.NewPhoneNumber).SetValidator(new PhoneNumberValidator());
    }
}