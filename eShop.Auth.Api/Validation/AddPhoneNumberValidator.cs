using eShop.Application.Validation;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Validation;

public class AddPhoneNumberValidator : Validator<AddPhoneNumberRequest>
{
    public AddPhoneNumberValidator()
    {
        RuleFor(model => model.PhoneNumber).SetValidator(new PhoneNumberValidator());
    }
}