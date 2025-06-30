using eShop.Domain.DTOs;

namespace eShop.Product.Api.Mapping;

public static class Mapper
{
    public static ProductTypeDto Map(ProductTypeEntity entity)
    {
        return new ProductTypeDto()
        {
            Id = entity.Id,
            Name = entity.Name
        };
    }
}