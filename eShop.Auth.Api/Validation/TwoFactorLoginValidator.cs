using eShop.Application.Validation;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Validation;

public class TwoFactorLoginValidator : Validator<TwoFactorLoginRequest>
{
    public TwoFactorLoginValidator()
    {
        RuleFor(x => x.Token).SetValidator(new CodeValidator());
    }
}