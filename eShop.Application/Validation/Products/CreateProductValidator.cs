using eShop.Domain.Requests.API.Product;

namespace eShop.Application.Validation.Products;

public class CreateProductValidator : Validator<CreateProductRequest>
{
    
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<CreateProductRequest>
            .CreateWithOptions((CreateProductRequest)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return [];
        return result.Errors.Select(e => e.ErrorMessage);
    };
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters long")
            .MaximumLength(100).WithMessage("Name must be no more than 100 characters");
        
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MinimumLength(8).WithMessage("Description must be at least 8 characters long")
            .MaximumLength(3000).WithMessage("Description must be no more than 3000 characters");
        
        RuleFor(x => x.Article)
            .NotEmpty().WithMessage("Article is required")
            .MinimumLength(9).WithMessage("Article must be at least 9 characters long")
            .MaximumLength(12).WithMessage("Article must be no more than 12 characters");
        
        RuleFor(x => x.Currency)
            .NotEqual(Currency.None).WithMessage("Choose currency");
            
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0")
            .LessThan(100_000).WithMessage("Price must be less than 100000");
        
        RuleFor(x => x.Images)
            .Must(images => images.Any()).WithMessage("Images are required")
            .When(x => x?.Images is not null);

        RuleFor(x => x.ProductType)
            .NotEqual(ProductTypes.None).WithMessage("Choose product type");

        RuleFor(x => x.Brand.Name)
            .NotEmpty().WithMessage("Name is required")
            .When(x => x?.Brand is not null);
        
        RuleFor(x => x.Brand.Id)
            .NotEmpty().WithMessage("Brand ID is required")
            .When(x => x?.Brand is not null);
        
        RuleFor(x => x.Size)
            .Must(x => !x.Contains(Size.None)).WithMessage("Size is required")
            .When(x => x.ProductType is ProductTypes.Shoes or ProductTypes.Clothing)
            .Must(x => x.Any()).WithMessage("Size is required")
            .When(x => x.ProductType switch
            {
                ProductTypes.Shoes => true,
                ProductTypes.Clothing => true,
                _ or ProductTypes.None => false,
            });
        
        RuleFor(x => x.Color)
            .NotEqual(ProductColor.None).WithMessage("Choose product color")
            .When(x => x.ProductType switch
            {
                ProductTypes.Shoes => true,
                ProductTypes.Clothing => true,
                _ or ProductTypes.None => false,
            });
        
        RuleFor(x => x.ProductAudience)
            .NotEqual(ProductAudience.None).WithMessage("Choose product audience")
            .When(x => x.ProductType switch
            {
                ProductTypes.Shoes => true,
                ProductTypes.Clothing => true,
                _ or ProductTypes.None => false,
            });
    }
}