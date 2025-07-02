using eShop.Application.Validation;
using eShop.BlazorWebUI.Models;

namespace eShop.BlazorWebUI.Validation;

public class ConfirmResetPhoneNumberValidator : Validator<ConfirmResetPhoneNumberModel>
{
    public ConfirmResetPhoneNumberValidator()
    {
        RuleFor(x => x.NewPhoneNumber).SetValidator(new PhoneNumberValidator());
    }
}