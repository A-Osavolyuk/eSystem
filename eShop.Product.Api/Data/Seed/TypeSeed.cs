using eShop.Domain.Abstraction.Data;

namespace eShop.Product.Api.Data.Seed;

public class TypeSeed : Seed<TypeEntity>
{
    public override List<TypeEntity> Get()
    {
        return
        [
            new TypeEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Fruit",
                Group = "Food",
                CreateDate = DateTimeOffset.UtcNow,
            },
            new TypeEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Vegetable",
                Group = "Food",
                CreateDate = DateTimeOffset.UtcNow,
            },
            new TypeEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Berry",
                Group = "Food",
                CreateDate = DateTimeOffset.UtcNow,
            },
        ];
    }
}