using eShop.Application.Validation;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Validation;

public class ConfirmChangeEmailValidator : Validator<ConfirmChangeEmailRequest>
{
    public ConfirmChangeEmailValidator()
    {
        RuleFor(x => x.Code).SetValidator(new CodeValidator());
    }
}