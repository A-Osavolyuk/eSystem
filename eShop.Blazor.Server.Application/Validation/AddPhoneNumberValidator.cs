using eShop.Blazor.Server.Domain.Models;

namespace eShop.Blazor.Server.Application.Validation;

public class AddPhoneNumberValidator : Validator<AddPhoneNumberModel>
{
    public AddPhoneNumberValidator()
    {
        RuleFor(model => model.PhoneNumber).SetValidator(new PhoneNumberValidator());
    }
}