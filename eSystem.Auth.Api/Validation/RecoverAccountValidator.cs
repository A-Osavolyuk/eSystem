using eSystem.Core.Requests.Auth;
using eSystem.Core.Validation;
using eSystem.Core.Validation.Validators;

namespace eSystem.Auth.Api.Validation;

public class RecoverAccountValidator : Validator<UnlockAccountRequest>
{
    public RecoverAccountValidator()
    {
        RuleFor(x => x.Code).SetValidator(new CodeValidator());
    }
}