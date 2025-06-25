using eShop.BlazorWebUI.Models;

namespace eShop.BlazorWebUI.Validation;

public class ConfirmResetEmailValidator : Validator<ConfirmResetEmailModel>
{
    public ConfirmResetEmailValidator()
    {
        RuleFor(x => x.Code)
            .Length(6).WithMessage("Code length must be 6");
    }
}