using eAccount.Blazor.Server.Domain.Models;

namespace eAccount.Blazor.Server.Application.Validation;

public class CodeModelValidator : Validator<CodeModel>
{
    public CodeModelValidator()
    {
        RuleFor(x => x.Code).SetValidator(new CodeValidator());
    }
}