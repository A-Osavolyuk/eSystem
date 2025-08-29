using eShop.Application.Validation;
using eShop.BlazorWebUI.Models;

namespace eShop.BlazorWebUI.Validation;

public class CodeModelValidator : Validator<CodeModel>
{
    public CodeModelValidator()
    {
        RuleFor(x => x.Code).SetValidator(new CodeValidator());
    }
}