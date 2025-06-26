using eShop.Application.Validation;
using eShop.BlazorWebUI.Models;

namespace eShop.BlazorWebUI.Validation;

public class VerifyCodeValidator : Validator<VerifyCodeModel>
{
    public VerifyCodeValidator()
    {
        RuleFor(x => x.Code).SetValidator(new CodeValidator());
    }
}