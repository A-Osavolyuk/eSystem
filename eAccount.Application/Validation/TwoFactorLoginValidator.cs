using eAccount.Domain.Models;
using eSystem.Core.Validation;
using eSystem.Core.Validation.Validators;

namespace eAccount.Application.Validation;

public class TwoFactorLoginValidator : Validator<TwoFactorLoginModel>
{
    public TwoFactorLoginValidator()
    {
        RuleFor(x => x.Code).SetValidator(new CodeValidator());
    }
}