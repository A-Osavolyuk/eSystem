using eShop.Domain.Abstraction.Data;

namespace eShop.Product.Api.Data.Seed;

public class TypeSeed : Seed<ProductTypeEntity>
{
    public override List<ProductTypeEntity> Get()
    {
        return
        [
            new ProductTypeEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "fruit",
                CreateDate = DateTimeOffset.UtcNow,
            },
            new ProductTypeEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "vegetable",
                CreateDate = DateTimeOffset.UtcNow,
            },
            new ProductTypeEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "berries",
                CreateDate = DateTimeOffset.UtcNow,
            },
        ];
    }
}