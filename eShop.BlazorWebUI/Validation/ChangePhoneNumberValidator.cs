using eShop.BlazorWebUI.Models;
using eShop.Domain.Requests.API.Auth;

namespace eShop.BlazorWebUI.Validation;

public class ChangePhoneNumberValidator : Validator<ChangePhoneNumberModel>
{
    public ChangePhoneNumberValidator()
    {
        RuleFor(x => x.NewPhoneNumber)
            .Matches(@"^\+\(\d{2}\)-\d{3}-\d{3}-\d{4}$|^\d{12}$").WithMessage("Wrong phone number format.")
            .NotEmpty().WithMessage("Phone number is must!");
    }
}