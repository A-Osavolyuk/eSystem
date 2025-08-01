using eShop.Application.Validation;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Validation;

public class ResetPasswordValidator : Validator<ConfirmForgotPasswordRequest>
{
    public ResetPasswordValidator()
    {
        RuleFor(p => p.Code).SetValidator(new CodeValidator());
    }
}