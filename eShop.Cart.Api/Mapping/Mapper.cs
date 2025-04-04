using eShop.Cart.Api.Entities;
using eShop.Domain.DTOs;
using eShop.Domain.Models;

namespace eShop.Cart.Api.Mapping;

public static class Mapper
{
    public static FavoritesDto Map(FavoritesEntity entity)
    {
        return new()
        {
            Items = entity.Items,
            Id = entity.Id,
            Count = entity.ItemsCount,
        };
    }

    public static CartDto Map(CartEntity entity)
    {
        return new()
        {
            Items = entity.Items,
            Id = entity.Id,
            Count = entity.ItemsCount,
        };
    }
}