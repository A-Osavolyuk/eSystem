using eShop.Application.Validation;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Validation;

public class ConfirmResetEmailValidator : Validator<ConfirmResetEmailRequest>
{
    public ConfirmResetEmailValidator()
    {
        RuleFor(x => x.Code).SetValidator(new CodeValidator());
    }
}