using eShop.Cart.Api.Entities;
using eShop.Domain.Types;
using Scalar.AspNetCore;

namespace eShop.Cart.Api.Extensions;

public static class WebApplicationExtensions
{
    public static async Task MapApiServices(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
            app.UseCors();
            await app.SeedDataAsync();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.UseExceptionHandler();
        app.MapGrpcService<CartServer>();
    }

    private static async Task SeedDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<DbClient>();
        var cartCollection = client.GetCollection<CartEntity>("Carts");
        var favoritesCollection = client.GetCollection<FavoritesEntity>("Favorites");

        await cartCollection.InsertOneAsync(new CartEntity()
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Parse("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3"),
            ItemsCount = 0,
            Items = [],
            UpdateDate = DateTime.UtcNow,
            CreateDate = DateTime.UtcNow
        });

        await favoritesCollection.InsertOneAsync(new FavoritesEntity()
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Parse("abb9d2ed-c3d2-4df9-ba88-eab018b95bc3"),
            ItemsCount = 0,
            Items = [],
            UpdateDate = DateTime.UtcNow,
            CreateDate = DateTime.UtcNow
        });
    }
}