using eShop.Blazor.Server.Domain.Models;

namespace eShop.Blazor.Server.Application.Validation;

public class CreatePasskeyValidator : Validator<CreatePasskeyModel>
{
    public CreatePasskeyValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Field is required")
            .MinimumLength(3).WithMessage("First name must be at least 3 characters long.");
    }
}