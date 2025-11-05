using eSecurity.Common.Models;
using eSystem.Core.Validation;
using eSystem.Core.Validation.Validators;

namespace eSecurity.Validation.Validators;

public class TwoFactorLoginValidator : Validator<TwoFactorLoginModel>
{
    public TwoFactorLoginValidator()
    {
        RuleFor(x => x.Code).SetValidator(new CodeValidator());
    }
}