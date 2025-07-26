using eShop.BlazorWebUI.Models;
using eShop.BlazorWebUI.Models.Products;

namespace eShop.BlazorWebUI.Validation.Products;

public abstract class PropertiesValidator<T> : AbstractValidator<T> where T : ProductPropertiesModel
{
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var productModel = (model as ProductModel)!;
        var properties = (productModel.Properties as T)!;
        
        var result = await ValidateAsync(ValidationContext<T>
            .CreateWithOptions(properties, x => x.IncludeProperties(propertyName)));
        
        return result.IsValid ? [] : result.Errors.Select(e => e.ErrorMessage);
    };
}