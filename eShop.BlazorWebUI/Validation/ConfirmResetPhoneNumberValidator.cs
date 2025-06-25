using eShop.BlazorWebUI.Models;

namespace eShop.BlazorWebUI.Validation;

public class ConfirmResetPhoneNumberValidator : Validator<ConfirmResetPhoneNumberModel>
{
    public ConfirmResetPhoneNumberValidator()
    {
        RuleFor(x => x.NewPhoneNumber)
            .Matches(@"^\+\(\d{2}\)-\d{3}-\d{3}-\d{4}$|^\d{12}$").WithMessage("Wrong phone number format.")
            .NotEmpty().WithMessage("Phone number is must!");
    }
}