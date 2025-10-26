using eSystem.Core.Requests.Auth;
using eSystem.Core.Validation;

namespace eSystem.Auth.Api.Validation;

public class ChangeEmailValidator : Validator<ChangeEmailRequest>
{
    public ChangeEmailValidator()
    {
        RuleFor(x => x.NewEmail).EmailAddress();
    }
}