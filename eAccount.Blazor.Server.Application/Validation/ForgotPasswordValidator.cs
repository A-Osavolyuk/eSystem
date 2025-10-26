using eAccount.Blazor.Server.Domain.Models;
using eSystem.Core.Validation;

namespace eAccount.Blazor.Server.Application.Validation;

public class ForgotPasswordValidator : Validator<ForgotPasswordModel>
{
    public ForgotPasswordValidator()
    {
        RuleFor(p => p.Email)
            .NotEmpty().WithMessage("Email is must.")
            .EmailAddress().WithMessage("Invalid format of email address.");
    }
}