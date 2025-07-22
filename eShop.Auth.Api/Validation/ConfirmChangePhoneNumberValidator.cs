using eShop.Application.Validation;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Validation;

public class ConfirmChangePhoneNumberValidator : Validator<ConfirmChangePhoneNumberRequest>
{
    public ConfirmChangePhoneNumberValidator()
    {
        RuleFor(x => x.Code).SetValidator(new CodeValidator());
    }
}