using eSystem.Domain.Requests.Auth;

namespace eSystem.Auth.Api.Validation;

public class ChangeEmailValidator : Validator<ChangeEmailRequest>
{
    public ChangeEmailValidator()
    {
        RuleFor(x => x.NewEmail).EmailAddress();
    }
}