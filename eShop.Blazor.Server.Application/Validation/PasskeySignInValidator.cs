using eShop.Blazor.Server.Domain.Models;

namespace eShop.Blazor.Server.Application.Validation;

public class PasskeySignInValidator : Validator<PasskeySignInModel>
{
    public PasskeySignInValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Please enter your username")
            .MinimumLength(3).WithMessage("Field must be at least 3 characters long")
            .MaximumLength(32).WithMessage("Field must be at most 32 characters long");
    }
}