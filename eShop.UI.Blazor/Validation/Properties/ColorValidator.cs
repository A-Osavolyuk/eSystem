namespace eShop.BlazorWebUI.Validation.Properties;

public class ColorValidator : AbstractValidator<string>
{
    public ColorValidator()
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage("Field is required.")
            .Length(4, 64).WithMessage("Field length must be between 4 and 64 characters.");
    }
}