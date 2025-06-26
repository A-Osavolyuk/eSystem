using eShop.Application.Validation;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Validation;

public class VerifyCodeValidator : Validator<VerifyCodeRequest>
{
    public VerifyCodeValidator()
    {
        RuleFor(x => x.Code).SetValidator(new CodeValidator());
    }
}