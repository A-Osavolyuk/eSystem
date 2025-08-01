using eShop.Application.Validation;
using eShop.BlazorWebUI.Models;

namespace eShop.BlazorWebUI.Validation;

public class VerifyNewEmailValidator : Validator<VerifyCurrentEmailModel>
{
    public VerifyNewEmailValidator()
    {
        RuleFor(x => x.Code).SetValidator(new CodeValidator());
    }
}