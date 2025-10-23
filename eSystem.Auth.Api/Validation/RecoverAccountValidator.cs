using eSystem.Application.Validation;
using eSystem.Domain.Requests.Auth;

namespace eSystem.Auth.Api.Validation;

public class RecoverAccountValidator : Validator<UnlockAccountRequest>
{
    public RecoverAccountValidator()
    {
        RuleFor(x => x.Code).SetValidator(new CodeValidator());
    }
}