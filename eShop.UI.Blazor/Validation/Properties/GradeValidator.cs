namespace eShop.BlazorWebUI.Validation.Properties;

public class GradeValidator : AbstractValidator<string>
{
    public GradeValidator()
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage("Field is required.")
            .Length(1, 32).WithMessage("Field length must be between 1 and 32 characters.");
    }
}