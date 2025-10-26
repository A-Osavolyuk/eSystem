using eAccount.Blazor.Server.Domain.Models;
using eSystem.Core.Validation;

namespace eAccount.Blazor.Server.Application.Validation;

public class AddEmailValidator : Validator<AddEmailModel>
{
    public AddEmailValidator()
    {
        RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid email address");
    }
}