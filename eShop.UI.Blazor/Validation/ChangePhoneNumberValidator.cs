using eShop.Application.Validation;
using eShop.BlazorWebUI.Models;

namespace eShop.BlazorWebUI.Validation;

public class ChangePhoneNumberValidator : Validator<ChangePhoneNumberModel>
{
    public ChangePhoneNumberValidator()
    {
        RuleFor(x => x.NewPhoneNumber).SetValidator(new PhoneNumberValidator());
    }
}