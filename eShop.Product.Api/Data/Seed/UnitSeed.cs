using eShop.Domain.Abstraction.Data;

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
                Code = "item"
            },
            new UnitEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Kilogram",
                Code = "kg"
            },
            new UnitEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Gram",
                Code = "g"
            },
            new UnitEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Liter",
                Code = "l"
            },
            new UnitEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Milliliter",
                Code = "ml"
            },
            new UnitEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Meter",
                Code = "m"
            },
            new UnitEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Centimeter",
                Code = "cm"
            },
            new UnitEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "SquareMeter",
                Code = "m2"
            },
            new UnitEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "CubicMeter",
                Code = "m3"
            },
            new UnitEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Dozen",
                Code = "d"
            },
            new UnitEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Package",
                Code = "pack"
            },
            new UnitEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Box",
                Code = "box"
            },
            new UnitEntity()
            {
                Id = Guid.CreateVersion7(),
                Name = "Piece",
                Code = "psc"
            }
        ];
    }
}