using eAccount.Blazor.Server.Domain.Models;
using eSystem.Core.Validation;

namespace eAccount.Blazor.Server.Application.Validation;

public class ChangeEmailValidator : Validator<ChangeEmailModel>
{
    public ChangeEmailValidator()
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty().WithMessage("New email is must!")
            .EmailAddress().WithMessage("Invalid email address format.");
    }
}