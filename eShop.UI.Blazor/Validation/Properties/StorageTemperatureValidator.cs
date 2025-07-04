namespace eShop.BlazorWebUI.Validation.Properties;

public class StorageTemperatureValidator : AbstractValidator<string>
{
    public StorageTemperatureValidator()
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage("Field is required")
            .Length(2, 64).WithMessage("Field length must be between 4 and 64 characters");
    }
}