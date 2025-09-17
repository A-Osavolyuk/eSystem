using eShop.Blazor.Server.Domain.Models;

namespace eShop.Blazor.Server.Application.Validation;

public class VerifyEmailValidator : Validator<VerifyEmailModel>
{
    public VerifyEmailValidator()
    {
        RuleFor(x => x.Code).SetValidator(new CodeValidator());
    }
}