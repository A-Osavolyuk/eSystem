using eSecurity.Common.Models;
using eSystem.Core.Validation;

namespace eSecurity.Validation.Validators;

public class CreatePasskeyValidator : Validator<CreatePasskeyModel>
{
    public CreatePasskeyValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Field is required")
            .MinimumLength(3).WithMessage("First name must be at least 3 characters long.");
    }
}