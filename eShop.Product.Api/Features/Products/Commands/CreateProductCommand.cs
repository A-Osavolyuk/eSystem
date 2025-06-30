using System.Text.Json;
using eShop.Domain.Requests.API.Product;

namespace eShop.Product.Api.Features.Product.Commands;

public record CreateProductCommand(CreateProductRequest Request) : IRequest<Result>;

public class CreateProductCommandHandler(
    IProductManager productManager,
    ITypeManager typeManager) : IRequestHandler<CreateProductCommand, Result>
{
    private readonly IProductManager productManager = productManager;
    private readonly ITypeManager typeManager = typeManager;

    public async Task<Result> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var type = await typeManager.FindByIdAsync(request.Request.TypeId, cancellationToken);

        if (type is null)
        {
            return Results.NotFound($"Cannot find product type with ID {request.Request.TypeId}");
        }
        
        var entity = Map(request.Request, type.Name);
        
        entity.Id = Guid.CreateVersion7();
        entity.Name = request.Request.Name;
        entity.TypeId = type.Id;
        entity.UnitId = request.Request.UnitId;
        entity.PriceTypeId = request.Request.PriceTypeId;
        entity.Price = request.Request.Price;
        entity.QuantityInStock = request.Request.QuantityInStock;
        entity.CreateDate = DateTimeOffset.UtcNow;

        var result = await productManager.CreateAsync(entity, cancellationToken);

        return result;
    }

    private ProductEntity Map(CreateProductRequest request, string type)
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

        var entity = type switch
        {
            "fruit" => JsonConvert.DeserializeObject<FruitProductEntity>(json)!,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        return entity;
    }
}