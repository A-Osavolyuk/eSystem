using eSystem.Core.Requests.Auth;
using eSystem.Core.Validation;

namespace eSecurity.Validation;

public class ChangeEmailValidator : Validator<ChangeEmailRequest>
{
    public ChangeEmailValidator()
    {
        RuleFor(x => x.NewEmail).EmailAddress();
    }
}