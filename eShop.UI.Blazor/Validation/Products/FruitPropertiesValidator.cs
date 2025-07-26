using eShop.BlazorWebUI.Models;
using eShop.BlazorWebUI.Models.Products;

namespace eShop.BlazorWebUI.Validation.Products;

public class FruitPropertiesValidator : PropertiesValidator<FruitPropertiesModel>
{
    public FruitPropertiesValidator()
    {
        RuleFor(x => x.Color)
            .NotEmpty().WithMessage("Field is required")
            .MinimumLength(3).WithMessage("Field must be at least 3 characters long")
            .MaximumLength(32).WithMessage("Field must be less than 32 characters long");
        
        RuleFor(x => x.CountryOfOrigin)
            .NotEmpty().WithMessage("Field is required")
            .MinimumLength(2).WithMessage("Field must be at least 2 characters long")
            .MaximumLength(64).WithMessage("Field must be less than 64 characters long");
        
        RuleFor(x => x.Grade)
            .NotEmpty().WithMessage("Field is required")
            .MinimumLength(1).WithMessage("Field must be at least 1 characters long")
            .MaximumLength(16).WithMessage("Field must be less than 16 characters long");
        
        RuleFor(x => x.RipenessStage)
            .NotEmpty().WithMessage("Field is required")
            .MinimumLength(3).WithMessage("Field must be at least 3 characters long")
            .MaximumLength(32).WithMessage("Field must be less than 32 characters long");
        
        RuleFor(x => x.Variety)
            .NotEmpty().WithMessage("Field is required")
            .MinimumLength(2).WithMessage("Field must be at least 2 characters long")
            .MaximumLength(64).WithMessage("Field must be less than 64 characters long");
        
        RuleFor(x => x.StorageTemperature)
            .NotEmpty().WithMessage("Field is required")
            .MinimumLength(1).WithMessage("Field must be at least 1 characters long")
            .MaximumLength(32).WithMessage("Field must be less than 32 characters long");
    }
}