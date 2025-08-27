using eShop.Application.Validation;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Validation;

public class ResetPhoneNumberValidator : Validator<ResetPhoneNumberRequest>
{
    public ResetPhoneNumberValidator()
    {
        RuleFor(x => x.PhoneNumber).SetValidator(new PhoneNumberValidator());
    }
}