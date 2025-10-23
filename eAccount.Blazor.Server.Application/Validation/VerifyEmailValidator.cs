using eAccount.Blazor.Server.Domain.Models;

namespace eAccount.Blazor.Server.Application.Validation;

public class VerifyEmailValidator : Validator<VerifyEmailModel>
{
    public VerifyEmailValidator()
    {
        RuleFor(x => x.Code).SetValidator(new CodeValidator());
    }
}