using eAccount.Blazor.Server.Domain.Models;

namespace eAccount.Blazor.Server.Application.Validation;

public class AddEmailValidator : Validator<AddEmailModel>
{
    public AddEmailValidator()
    {
        RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid email address");
    }
}