using eShop.Cart.Api.Entities;
using eShop.Domain.DTOs;
using eShop.Domain.Models;

namespace eShop.Cart.Api.Mapping;

public static class Mapper
{
    public static FavoritesDto ToFavoritesDto(FavoritesEntity entity)
    {
        return new()
        {
            Items = entity.Items,
            Id = entity.Id,
            Count = entity.ItemsCount,
        };
    }

    public static FavoritesModel ToFavoritesModel(FavoritesDto dto)
    {
        return new()
        {
            Id = dto.Id,
            Items = dto.Items,
            ItemsCount = dto.Count,
        };
    }

    public static CartDto ToCartDto(CartEntity entity)
    {
        return new()
        {
            Items = entity.Items,
            Id = entity.Id,
            Count = entity.ItemsCount,
        };
    }
}