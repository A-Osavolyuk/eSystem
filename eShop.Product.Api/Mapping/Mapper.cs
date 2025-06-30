using eShop.Domain.DTOs;

namespace eShop.Product.Api.Mapping;

public static class Mapper
{
    public static TypeDto Map(TypeEntity entity)
    {
        return new TypeDto()
        {
            Id = entity.Id,
            Name = entity.Name,
            Category = Map(entity.Category!)
        };
    }
    
    public static CategoryDto Map(CategoryEntity entity)
    {
        return new CategoryDto()
        {
            Id = entity.Id,
            Name = entity.Name
        };
    }
    
    public static PriceTypeDto Map(PriceTypeEntity entity)
    {
        return new PriceTypeDto()
        {
            Id = entity.Id,
            Name = entity.Name
        };
    }
    
    public static UnitDto Map(UnitEntity entity)
    {
        return new UnitDto()
        {
            Id = entity.Id,
            Name = entity.Name,
            Code = entity.Code
        };
    }
}