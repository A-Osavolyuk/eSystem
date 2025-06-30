namespace eShop.Product.Api.Data.Seed;

public class PriceTypeSeed : Seed<PriceTypeEntity>
{
    public override List<PriceTypeEntity> Get()
    {
        return
        [
            new PriceTypeEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Per item",
                CreateDate = DateTimeOffset.UtcNow
            },
            new PriceTypeEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Per kilogram",
                CreateDate = DateTimeOffset.UtcNow
            },
            new PriceTypeEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Per gram",
                CreateDate = DateTimeOffset.UtcNow
            },
            new PriceTypeEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Per liter",
                CreateDate = DateTimeOffset.UtcNow
            },
            new PriceTypeEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Per milliliter",
                CreateDate = DateTimeOffset.UtcNow
            },
            new PriceTypeEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Per meter",
                CreateDate = DateTimeOffset.UtcNow
            },
            new PriceTypeEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Per centimeter",
                CreateDate = DateTimeOffset.UtcNow
            },
            new PriceTypeEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Per square meter",
                CreateDate = DateTimeOffset.UtcNow
            },
            new PriceTypeEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Per cubic meter",
                CreateDate = DateTimeOffset.UtcNow
            },
            new PriceTypeEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Per dozen",
                CreateDate = DateTimeOffset.UtcNow
            },
            new PriceTypeEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Per package",
                CreateDate = DateTimeOffset.UtcNow
            },
            new PriceTypeEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Per box",
                CreateDate = DateTimeOffset.UtcNow
            },
            new PriceTypeEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Per piece",
                CreateDate = DateTimeOffset.UtcNow
            },
        ];
    }
}