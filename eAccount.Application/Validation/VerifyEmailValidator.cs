using eAccount.Domain.Models;
using eSystem.Core.Validation;
using eSystem.Core.Validation.Validators;

namespace eAccount.Application.Validation;

public class VerifyEmailValidator : Validator<VerifyEmailModel>
{
    public VerifyEmailValidator()
    {
        RuleFor(x => x.Code).SetValidator(new CodeValidator());
    }
}