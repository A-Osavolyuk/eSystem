namespace eShop.Product.Api.Data.Seed;

public class UnitSeed : Seed<UnitEntity>
{
    public override List<UnitEntity> Get()
    {
        return
        [
            new UnitEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Item",
                Code = "item",
                CreateDate = DateTimeOffset.UtcNow
            },
            new UnitEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Kilogram",
                Code = "kg",
                CreateDate = DateTimeOffset.UtcNow
            },
            new UnitEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Gram",
                Code = "g",
                CreateDate = DateTimeOffset.UtcNow
            },
            new UnitEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Liter",
                Code = "l",
                CreateDate = DateTimeOffset.UtcNow
            },
            new UnitEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Milliliter",
                Code = "ml",
                CreateDate = DateTimeOffset.UtcNow
            },
            new UnitEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Meter",
                Code = "m",
                CreateDate = DateTimeOffset.UtcNow
            },
            new UnitEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Centimeter",
                Code = "cm",
                CreateDate = DateTimeOffset.UtcNow
            },
            new UnitEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "SquareMeter",
                Code = "m2",
                CreateDate = DateTimeOffset.UtcNow
            },
            new UnitEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "CubicMeter",
                Code = "m3",
                CreateDate = DateTimeOffset.UtcNow
            },
            new UnitEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Dozen",
                Code = "d",
                CreateDate = DateTimeOffset.UtcNow
            },
            new UnitEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Package",
                Code = "pack",
                CreateDate = DateTimeOffset.UtcNow
            },
            new UnitEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Box",
                Code = "box",
                CreateDate = DateTimeOffset.UtcNow
            },
            new UnitEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Piece",
                Code = "psc",
                CreateDate = DateTimeOffset.UtcNow
            }
        ];
    }
}