using eAccount.Blazor.Server.Domain.Models;
using eSystem.Core.Validation;
using eSystem.Core.Validation.Validators;

namespace eAccount.Blazor.Server.Application.Validation;

public class TwoFactorLoginValidator : Validator<TwoFactorLoginModel>
{
    public TwoFactorLoginValidator()
    {
        RuleFor(x => x.Code).SetValidator(new CodeValidator());
    }
}