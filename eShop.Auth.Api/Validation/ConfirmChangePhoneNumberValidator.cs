using eShop.Application.Validation;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Validation;

public class ConfirmChangePhoneNumberValidator : Validator<ConfirmChangePhoneNumberRequest>
{
    public ConfirmChangePhoneNumberValidator()
    {
        RuleFor(x => x.CurrentPhoneNumberCode).SetValidator(new CodeValidator());
        RuleFor(x => x.NewPhoneNumberCode).SetValidator(new CodeValidator());
    }
}