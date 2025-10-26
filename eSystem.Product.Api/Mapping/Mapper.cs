using eSystem.Core.DTOs;
using eSystem.Product.Api.Entities;

namespace eSystem.Product.Api.Mapping;

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
    
    public static CurrencyDto Map(CurrencyEntity entity)
    {
        return new CurrencyDto()
        {
            Id = entity.Id,
            Name = entity.Name,
            Code = entity.Code,
            Sign = entity.Sign
        };
    }
    
    public static BrandDto Map(BrandEntity entity)
    {
        return new BrandDto()
        {
            Id = entity.Id,
            Name = entity.Name,
            Country = entity.Country,
            Description = entity.Description,
            LogoUrl = entity.LogoUrl,
            WebsiteUrl = entity.WebsiteUrl
        };
    }
    
    public static SupplierDto Map(SupplierEntity entity)
    {
        return new SupplierDto()
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            WebsiteUrl = entity.WebsiteUrl,
            Address = entity.Address,
            Email = entity.Email,
            PhoneNumber = entity.PhoneNumber
        };
    }
}