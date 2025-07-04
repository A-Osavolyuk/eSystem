namespace eShop.BlazorWebUI.Validation.Properties;

public class OriginCountryValidator : AbstractValidator<string>
{
    public OriginCountryValidator()
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage("Field is required.")
            .Length(2, 64).WithMessage("Field length must be between 2 and 64 characters.");
    }
}