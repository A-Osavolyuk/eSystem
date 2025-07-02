using eShop.Application.Validation;
using eShop.BlazorWebUI.Models;

namespace eShop.BlazorWebUI.Validation;

public class ConfirmChangeEmailValidator : Validator<ConfirmChangeEmailModel>
{
    public ConfirmChangeEmailValidator()
    {
        RuleFor(x => x.CurrentEmailCode).SetValidator(new CodeValidator());
        RuleFor(x => x.NewEmailCode).SetValidator(new CodeValidator());
    }
}