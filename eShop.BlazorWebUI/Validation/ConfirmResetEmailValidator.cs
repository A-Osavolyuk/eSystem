using eShop.Application.Validation;
using eShop.BlazorWebUI.Models;

namespace eShop.BlazorWebUI.Validation;

public class ConfirmResetEmailValidator : Validator<ConfirmResetEmailModel>
{
    public ConfirmResetEmailValidator()
    {
        RuleFor(x => x.Code).SetValidator(new CodeValidator());
    }
}