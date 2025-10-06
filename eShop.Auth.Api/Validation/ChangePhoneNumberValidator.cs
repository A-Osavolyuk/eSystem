using eShop.Application.Validation;
using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Validation;

public class ChangePhoneNumberValidator : Validator<ChangePhoneNumberRequest>
{
    public ChangePhoneNumberValidator()
    {
        RuleFor(x => x.NewPhoneNumber).SetValidator(new PhoneNumberValidator());
    }
}