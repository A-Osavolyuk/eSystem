using eShop.Application.Validation;
using eShop.BlazorWebUI.Models;

namespace eShop.BlazorWebUI.Validation;

public class ResetPhoneNumberValidator : Validator<ResetPhoneNumberModel>
{
    public ResetPhoneNumberValidator()
    {
        RuleFor(x => x.NewPhoneNumber).SetValidator(new PhoneNumberValidator());
    }
}