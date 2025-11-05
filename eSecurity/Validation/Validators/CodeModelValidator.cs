using eSecurity.Common.Models;
using eSystem.Core.Validation;
using eSystem.Core.Validation.Validators;

namespace eSecurity.Validation.Validators;

public class CodeModelValidator : Validator<CodeModel>
{
    public CodeModelValidator()
    {
        RuleFor(x => x.Code).SetValidator(new CodeValidator());
    }
}