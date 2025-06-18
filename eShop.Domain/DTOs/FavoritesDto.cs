using eShop.Domain.Abstraction.Data;

namespace eShop.Domain.DTOs;

public class FavoritesDto
{
    public Guid Id { get; init; }
    public int Count { get; set; }
    public List<FavoritesItem> Items { get; set; } = [];
}