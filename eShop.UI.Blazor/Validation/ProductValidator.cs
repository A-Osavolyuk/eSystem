using eShop.BlazorWebUI.Models;
using eShop.BlazorWebUI.Validation.Properties;

namespace eShop.BlazorWebUI.Validation;

public class ProductValidator : Validator<ProductModel>
{
    public ProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Field is required")
            .MaximumLength(128).WithMessage("Name must be less than 128 characters.")
            .MinimumLength(4).WithMessage("Name must be less than 4 characters.");
        
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Field is required")
            .MaximumLength(3000).WithMessage("Description must be less than 3000 characters.")
            .MinimumLength(4).WithMessage("Description must be less than 4 characters.");
        
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.")
            .LessThan(100_000).WithMessage("Price must be less than 100000.");
        
        RuleFor(x => x.QuantityInStock)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.")
            .LessThan(100_000).WithMessage("Quantity must be less than 100000.");

        RuleFor(x => x.PriceType)
            .NotEmpty().WithMessage("Field is required");
        
        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Field is required");
        
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Field is required");
        
        RuleFor(x => x.Unit)
            .NotEmpty().WithMessage("Field is required");
    }
}