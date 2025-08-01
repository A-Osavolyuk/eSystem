using eShop.Application.Validation;
using eShop.BlazorWebUI.Models;

namespace eShop.BlazorWebUI.Validation;

public class VerifyNewPhoneNumberValidator : Validator<VerifyCurrentPhoneNumberModel>
{
    public VerifyNewPhoneNumberValidator()
    {
        RuleFor(x => x.Code).SetValidator(new CodeValidator());
    }
}