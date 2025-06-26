using eShop.Application.Validation;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Validation;

public class ConfirmChangeEmailValidator : Validator<ConfirmChangeEmailRequest>
{
    public ConfirmChangeEmailValidator()
    {
        RuleFor(x => x.CurrentEmailCode).SetValidator(new CodeValidator());
        RuleFor(x => x.NewEmailCode).SetValidator(new CodeValidator());
    }
}