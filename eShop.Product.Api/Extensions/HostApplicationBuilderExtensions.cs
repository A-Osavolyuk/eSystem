using System.Text.Json.Serialization.Metadata;
using eShop.Domain.Interfaces.API;
using eShop.Domain.Requests.API.Product;
using eShop.Product.Api.Interfaces;
using eShop.Product.Api.Services;

namespace eShop.Product.Api.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static void AddApiServices(this IHostApplicationBuilder builder)
    {
        builder.AddLogging();
        builder.AddServiceDefaults();
        builder.AddJwtAuthentication();
        builder.AddDependencyInjection();
        builder.AddVersioning();
        builder.AddMessageBus();
        builder.AddValidation();
        builder.AddRedisCache();
        builder.AddMediatR();
        builder.AddMsSqlDb();
        builder.AddExceptionHandler();
        builder.AddDocumentation();
        
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers =
                {
                    resolver =>
                    {
                        if (resolver.Type == typeof(CreateProductRequest))
                        {
                            var polymorphism = new JsonPolymorphismOptions
                            {
                                TypeDiscriminatorPropertyName = "productType"
                            };
                            polymorphism.DerivedTypes.Add(new JsonDerivedType(typeof(CreateFruitProductRequest), (int)ProductType.Fruit));
                            resolver.PolymorphismOptions = polymorphism;
                        }
                    }
                }
            };
        });
    }

    private static void AddMsSqlDb(this IHostApplicationBuilder builder)
    {
        builder.AddSqlServerDbContext<AppDbContext>("product-db");
    }
    
    private static void AddValidation(this IHostApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>();
    }
    
    private static void AddRedisCache(this IHostApplicationBuilder builder)
    {
        builder.AddRedisClient("redis");
    }

    private static void AddMediatR(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediatR(c =>
        {
            c.RegisterServicesFromAssemblyContaining<IAssemblyMarker>();
            c.AddOpenBehavior(typeof(TransactionBehaviour<,>));
        });
    }
    private static void AddDependencyInjection(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ICacheService, CacheService>();
        builder.Services.AddScoped<IProductManager, ProductManager>();
    }

    private static void AddMessageBus(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var connectionString = builder.Configuration.GetConnectionString("rabbit-mq");
                cfg.Host(connectionString);
            });
        });
    }
}