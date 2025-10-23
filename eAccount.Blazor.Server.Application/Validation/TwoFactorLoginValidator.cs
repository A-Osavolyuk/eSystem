using eAccount.Blazor.Server.Domain.Models;

namespace eAccount.Blazor.Server.Application.Validation;

public class TwoFactorLoginValidator : Validator<TwoFactorLoginModel>
{
    public TwoFactorLoginValidator()
    {
        RuleFor(x => x.Code).SetValidator(new CodeValidator());
    }
}