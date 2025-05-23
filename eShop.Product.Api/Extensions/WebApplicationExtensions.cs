using eShop.Product.Api.Entities;
using Scalar.AspNetCore;

namespace eShop.Product.Api.Extensions;

public static class WebApplicationExtensions
{
    public static async Task MapApiServices(this WebApplication app)
    {
        app.MapDefaultEndpoints();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
            await app.ConfigureDatabaseAsync<AppDbContext>();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.ConfigureMongoDb();
        app.UseExceptionHandler();
    }

    private static void ConfigureMongoDb(this WebApplication app)
    {
        RegisterClassMaps();
    }

    private static void RegisterClassMaps()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(ProductEntity)))
        {
            BsonClassMap.RegisterClassMap<ProductEntity>(cm =>
            {
                cm.AutoMap();
                cm.SetIsRootClass(true);
                cm.SetDiscriminator("Product");
            });

            BsonClassMap.RegisterClassMap<ShoesEntity>(cm =>
            {
                cm.AutoMap();
                cm.SetDiscriminator("Shoes");
            });
            BsonClassMap.RegisterClassMap<ClothingEntity>(cm =>
            {
                cm.AutoMap();
                cm.SetDiscriminator("Clothing");
            });
        }
    }
}