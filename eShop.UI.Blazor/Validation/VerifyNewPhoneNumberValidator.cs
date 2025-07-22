using eShop.Application.Validation;
using eShop.BlazorWebUI.Models;

namespace eShop.BlazorWebUI.Validation;

public class VerifyNewPhoneNumberValidator : Validator<VerifyNewPhoneNumberModel>
{
    public VerifyNewPhoneNumberValidator()
    {
        RuleFor(x => x.Code).SetValidator(new CodeValidator());
    }
}