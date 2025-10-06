using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Validation;

public class ChangeEmailValidator : Validator<ChangeEmailRequest>
{
    public ChangeEmailValidator()
    {
        RuleFor(x => x.NewEmail).EmailAddress();
    }
}