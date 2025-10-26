using eAccount.Domain.Models;
using eSystem.Core.Validation;
using eSystem.Core.Validation.Validators;

namespace eAccount.Application.Validation;

public class CodeModelValidator : Validator<CodeModel>
{
    public CodeModelValidator()
    {
        RuleFor(x => x.Code).SetValidator(new CodeValidator());
    }
}