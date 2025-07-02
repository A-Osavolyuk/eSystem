using eShop.Application.Validation;
using eShop.BlazorWebUI.Models;

namespace eShop.BlazorWebUI.Validation;

public class VerifyEmailValidator : Validator<VerifyEmailModel>
{
    public VerifyEmailValidator()
    {
        RuleFor(x => x.Code).SetValidator(new CodeValidator());
    }
}