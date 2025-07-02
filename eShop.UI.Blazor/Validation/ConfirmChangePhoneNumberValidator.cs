using eShop.Application.Validation;
using eShop.BlazorWebUI.Models;

namespace eShop.BlazorWebUI.Validation;

public class ConfirmChangePhoneNumberValidator : Validator<ConfirmChangePhoneNumberModel>
{
    public ConfirmChangePhoneNumberValidator()
    {
        RuleFor(x => x.CurrentPhoneNumberCode).SetValidator(new CodeValidator());
        RuleFor(x => x.NewPhoneNumberCode).SetValidator(new CodeValidator());
    }
}