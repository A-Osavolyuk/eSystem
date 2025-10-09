using eShop.Application.Validation;
using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Validation;

public class TwoFactorLoginValidator : Validator<AuthenticatorSignInRequest>
{
    public TwoFactorLoginValidator()
    {
        RuleFor(x => x.Code).SetValidator(new CodeValidator());
    }
}