using eShop.Application.Validation;
using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Validation;

public class RecoverAccountValidator : Validator<UnlockAccountRequest>
{
    public RecoverAccountValidator()
    {
        RuleFor(x => x.Code).SetValidator(new CodeValidator());
    }
}