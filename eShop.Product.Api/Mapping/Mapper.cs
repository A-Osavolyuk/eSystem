using eShop.Domain.DTOs;

namespace eShop.Product.Api.Mapping;

public static class Mapper
{
    public static ProductTypeDto Map(TypeEntity entity)
    {
        return new ProductTypeDto()
        {
            Id = entity.Id,
            Name = entity.Name
        };
    }
}