using System.Text.Json;
using eShop.Domain.Requests.API.Product;
using eShop.Product.Api.Interfaces;

namespace eShop.Product.Api.Features.Product;

public record CreateProductCommand(CreateProductRequest Request) : IRequest<Result>;

public class CreateProductCommandHandler(IProductManager productManager) : IRequestHandler<CreateProductCommand, Result>
{
    private readonly IProductManager productManager = productManager;

    public async Task<Result> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var entity = Map(request.Request);
        
        entity.Id = Guid.CreateVersion7();
        entity.Name = request.Request.Name;
        entity.ProductType = request.Request.ProductType;
        entity.UnitOfMeasure = request.Request.UnitOfMeasure;
        entity.PricePerUnitType = request.Request.PricePerUnitType;
        entity.Price = request.Request.Price;
        entity.QuantityInStock = request.Request.QuantityInStock;
        entity.CreateDate = DateTimeOffset.UtcNow;

        var result = await productManager.CreateAsync(entity, cancellationToken);

        return result;
    }

    private ProductEntity Map(CreateProductRequest request)
    {
        var values = new Dictionary<string, object>();
        var properties = request.Properties.ToDictionary(
            x => x.Key, 
            x => (JsonElement)x.Value);

        foreach (var property in properties)
        {
            var value = property.Value.ValueKind switch
            {
                JsonValueKind.String => property.Value.GetString(),
                JsonValueKind.Number => property.Value.GetInt32(),
                JsonValueKind.True or JsonValueKind.False => property.Value.GetBoolean(),
                JsonValueKind.Object or JsonValueKind.Array => JsonConvert.DeserializeObject(property.Value.GetString()!),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            values.Add(property.Key, value!);
        }

        var json = JsonConvert.SerializeObject(values);

        var entity = request.ProductType switch
        {
            ProductType.Fruit => JsonConvert.DeserializeObject<FruitProductEntity>(json)!,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        return entity;
    }
}