namespace eShop.Application.Validation;

public class PhoneNumberValidator : AbstractValidator<string>
{
    public PhoneNumberValidator()
    {
        RuleFor(x => x)
            .Matches(@"^\+\(\d{2}\)-\d{3}-\d{3}-\d{4}$|^\d{12}$").WithMessage("Wrong phone number format. Must be: +(xx)-xxx-xxx-xxxx")
            .NotEmpty().WithMessage("Field is must!");
    }
}