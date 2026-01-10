using FluentValidation;

namespace eSystem.Core.Validation.Validators;

public class PhoneNumberValidator : AbstractValidator<string>
{
    public PhoneNumberValidator()
    {
        RuleFor(x => x)
            .Matches(@"^\+\d{1,3}\s\(\d{3}\)-\d{3}-\d{4}$").WithMessage("Wrong phone number format. Must be: +xx (xxx)-xxx-xxxx")
            .NotEmpty().WithMessage("Field is must!");
    }
}