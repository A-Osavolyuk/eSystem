using eShop.Application.Validation;
using eShop.BlazorWebUI.Models;

namespace eShop.BlazorWebUI.Validation;

public class VerifyNewEmailValidator : Validator<VerifyNewEmailModel>
{
    public VerifyNewEmailValidator()
    {
        RuleFor(x => x.Code).SetValidator(new CodeValidator());
    }
}