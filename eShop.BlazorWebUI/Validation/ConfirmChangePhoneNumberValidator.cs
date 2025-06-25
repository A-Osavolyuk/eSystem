using eShop.BlazorWebUI.Models;

namespace eShop.BlazorWebUI.Validation;

public class ConfirmChangePhoneNumberValidator : Validator<ConfirmChangePhoneNumberModel>
{
    public ConfirmChangePhoneNumberValidator()
    {
        RuleFor(x => x.CurrentPhoneNumberCode)
            .Length(6).WithMessage("Invalid code format");
        
        RuleFor(x => x.NewPhoneNumberCode)
            .Length(6).WithMessage("Invalid code format");
    }
}