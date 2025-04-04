using eShop.Domain.DTOs;
using eShop.Product.Api.Entities;

namespace eShop.Product.Api.Mapping;

public static class Mapper
{
    #region Create request to entities

    public static ClothingEntity ToClothingEntity(CreateProductRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        return new ClothingEntity()
        {
            Id = request.Id,
            Name = request.Name,
            Price = request.Price,
            ProductCurrency = request.ProductCurrency,
            ProductAudience = request.ProductAudience,
            ProductType = request.ProductType,
            Article = request.Article,
            Brand = Map(request.Brand),
            Seller = Map(request.Seller),
            Color = request.Color,
            Description = request.Description,
            Images = request.Images,
            Size = request.Size.ToList(),
        };
    }

    public static ShoesEntity ToShoesEntity(CreateProductRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        return new ShoesEntity()
        {
            Id = request.Id,
            Name = request.Name,
            Price = request.Price,
            ProductCurrency = request.ProductCurrency,
            ProductAudience = request.ProductAudience,
            ProductType = request.ProductType,
            Article = request.Article,
            Brand = Map(request.Brand),
            Seller = Map(request.Seller),
            Color = request.Color,
            Description = request.Description,
            Images = request.Images,
            Size = request.Size.ToList(),
        };
    }

    public static ProductEntity ToProductEntity(CreateProductRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        return new ProductEntity()
        {
            Id = request.Id,
            Name = request.Name,
            Price = request.Price,
            ProductCurrency = request.ProductCurrency,
            ProductType = request.ProductType,
            Article = request.Article,
            Brand = Map(request.Brand),
            Seller = Map(request.Seller),
            Description = request.Description,
            Images = request.Images,
        };
    }

    #endregion

    #region Update request to entities

    public static ClothingEntity ToClothingEntity(UpdateProductRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        return new ClothingEntity()
        {
            Name = request.Name,
            Price = request.Price,
            ProductCurrency = request.ProductCurrency,
            ProductAudience = request.ProductAudience,
            ProductType = request.ProductType,
            Article = request.Article,
            Brand = Map(request.Brand),
            Seller = Map(request.Seller),
            Color = request.Color,
            Description = request.Description,
            Images = request.Images,
            Size = request.Size,
        };
    }

    public static ShoesEntity ToShoesEntity(UpdateProductRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        return new ShoesEntity()
        {
            Name = request.Name,
            Price = request.Price,
            ProductCurrency = request.ProductCurrency,
            ProductAudience = request.ProductAudience,
            ProductType = request.ProductType,
            Article = request.Article,
            Brand = Map(request.Brand),
            Seller = Map(request.Seller),
            Color = request.Color,
            Description = request.Description,
            Images = request.Images,
            Size = request.Size,
        };
    }

    public static ProductEntity ToProductEntity(UpdateProductRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        return new ProductEntity()
        {
            Name = request.Name,
            Price = request.Price,
            ProductCurrency = request.ProductCurrency,
            ProductType = request.ProductType,
            Article = request.Article,
            Brand = Map(request.Brand),
            Seller = Map(request.Seller),
            Description = request.Description,
            Images = request.Images,
        };
    }

    #endregion

    #region Entities to DTOs

    public static ProductDto ToProductDto(ProductEntity entity)
    {
        return new ProductDto()
        {
            Id = entity.Id,
            Article = entity.Article,
            Brand = Map(entity.Brand),
            Seller = Map(entity.Seller),
            Description = entity.Description,
            Images = entity.Images,
            Type = entity.ProductType,
            Name = entity.Name,
            Price = entity.Price,
            Currency = entity.ProductCurrency,
        };
    }

    public static ClothingDto ToClothingDto(ClothingEntity entity)
    {
        return new ClothingDto()
        {
            Id = entity.Id,
            Article = entity.Article,
            Brand = Map(entity.Brand),
            Seller = Map(entity.Seller),
            Description = entity.Description,
            Images = entity.Images,
            Type = entity.ProductType,
            Name = entity.Name,
            Price = entity.Price,
            Currency = entity.ProductCurrency,
            ProductAudience = entity.ProductAudience,
            Color = entity.Color,
            Size = entity.Size,
        };
    }

    public static ShoesDto ToShoesDto(ShoesEntity entity)
    {
        return new ShoesDto()
        {
            Id = entity.Id,
            Article = entity.Article,
            Brand = Map(entity.Brand),
            Seller = Map(entity.Seller),
            Description = entity.Description,
            Images = entity.Images,
            Type = entity.ProductType,
            Name = entity.Name,
            Price = entity.Price,
            ProductAudience = entity.ProductAudience,
            Color = entity.Color,
            Size = entity.Size,
        };
    }

    #endregion

    public static BrandEntity Map(CreateBrandRequest request)
    {
        return new()
        {
            Name = request.Name,
            Country = request.Country,
        };
    }

    public static BrandEntity Map(UpdateBrandRequest request)
    {
        return new()
        {
            Name = request.Name,
            Country = request.Country,
        };
    }

    public static BrandDto Map(BrandEntity entity)
    {
        return new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Country = entity.Country,
        };
    }

    public static BrandEntity Map(BrandDto dto)
    {
        return new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Country = dto.Country,
        };
    }

    public static SellerDto Map(SellerEntity entity)
    {
        return new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Article = entity.Article,
            Email = entity.Email,
            PhoneNumber = entity.PhoneNumber,
            UserId = entity.UserId,
        };
    }

    public static SellerEntity Map(SellerDto dto)
    {
        return new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Article = dto.Article,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            UserId = dto.UserId,
        };
    }
}