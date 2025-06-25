using eShop.BlazorWebUI.Models;

namespace eShop.BlazorWebUI.Validation;

public class VerifyCodeValidator : Validator<VerifyCodeModel>
{
    public VerifyCodeValidator()
    {
        RuleFor(x => x.Code)
            .Length(6).WithMessage("Invalid code format");
    }
}