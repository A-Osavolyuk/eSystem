using FluentValidation;

namespace eSystem.Core.Validation.Validators;

public class CodeValidator : AbstractValidator<string>
{
    public CodeValidator()
    {
        RuleFor(x => x)
            .Length(6).WithMessage("Invalid code format.")
            .Matches(@"^\d+$").WithMessage("Only digits are allowed.");;
    }
}