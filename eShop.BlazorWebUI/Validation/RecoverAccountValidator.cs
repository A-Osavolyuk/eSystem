using eShop.BlazorWebUI.Models;

namespace eShop.BlazorWebUI.Validation;

public class RecoverAccountValidator : Validator<RecoverAccountModel>
{
    public RecoverAccountValidator()
    {
        RuleFor(x => x.Code)
            .Length(6).WithMessage("Invalid code format");
    }
}