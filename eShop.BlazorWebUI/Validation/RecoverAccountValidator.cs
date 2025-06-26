using eShop.Application.Validation;
using eShop.BlazorWebUI.Models;

namespace eShop.BlazorWebUI.Validation;

public class RecoverAccountValidator : Validator<RecoverAccountModel>
{
    public RecoverAccountValidator()
    {
        RuleFor(x => x.Code).SetValidator(new CodeValidator());
    }
}