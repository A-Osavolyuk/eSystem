using eAccount.Common.Models;
using eSystem.Core.Validation;

namespace eAccount.Validation.Validators;

public class ChangeUsernameValidator : Validator<ChangeUsernameModel>
{
    public ChangeUsernameValidator()
    {
        RuleFor(model => model.Username)
            .NotEmpty().WithMessage("Field is required.")
            .MaximumLength(32).WithMessage("Field must not exceed 32 characters.")
            .MinimumLength(3).WithMessage("Field must not exceed 3 characters.");
    }
}