using eShop.BlazorWebUI.Models;

namespace eShop.BlazorWebUI.Validation;

public class ResetPhoneNumberValidator : Validator<ResetPhoneNumberModel>
{
    public ResetPhoneNumberValidator()
    {
        RuleFor(x => x.NewPhoneNumber)
            .NotEmpty().WithMessage("Field is required.")
            .Matches(@"^\+\(\d{2}\)-\d{3}-\d{3}-\d{4}$|^\d{12}$").WithMessage("Wrong phone number format.");
    }
}