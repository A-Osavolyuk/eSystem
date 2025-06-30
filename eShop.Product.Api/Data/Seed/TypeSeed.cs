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
                CategoryId = Guid.Parse("b08c35a4-fc66-43f7-a98f-35e51d149d29"),
                Name = "Fruit",
                CreateDate = DateTimeOffset.UtcNow,
            },
            new TypeEntity()
            {
                Id = Guid.CreateVersion7(),
                CategoryId = Guid.Parse("b08c35a4-fc66-43f7-a98f-35e51d149d29"),
                Name = "Vegetable",
                CreateDate = DateTimeOffset.UtcNow,
            },
            new TypeEntity()
            {
                Id = Guid.CreateVersion7(),
                CategoryId = Guid.Parse("b08c35a4-fc66-43f7-a98f-35e51d149d29"),
                Name = "Berry",
                CreateDate = DateTimeOffset.UtcNow,
            },
        ];
    }
}