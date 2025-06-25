using eShop.BlazorWebUI.Models;

namespace eShop.BlazorWebUI.Validation;

public class AddPhoneNumberValidator : Validator<AddPhoneNumberModel>
{
    public AddPhoneNumberValidator()
    {
        RuleFor(model => model.PhoneNumber)
            .Matches(@"^\+\(\d{2}\)-\d{3}-\d{3}-\d{4}$|^\d{12}$").WithMessage("Wrong phone number format.")
            .NotEmpty().WithMessage("Phone number is must!");
    }
}