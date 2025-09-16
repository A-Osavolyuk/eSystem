using eShop.Application.Security.Authorization.Requirements;

namespace eShop.Product.Api.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static void AddApiServices(this IHostApplicationBuilder builder)
    {
        builder.AddLogging();
        builder.AddServiceDefaults();
        builder.AddJwtAuthentication();
        builder.AddAuthorization();
        builder.AddVersioning();
        builder.AddMessageBus();
        builder.AddValidation<IAssemblyMarker>();
        builder.AddRedisCache();
        builder.AddMediatR();
        builder.AddMsSqlDb();
        builder.AddExceptionHandler();
        builder.AddDocumentation();
        builder.AddServices<IAssemblyMarker>();

        builder.Services.AddControllers();
    }

    private static void AddMsSqlDb(this IHostApplicationBuilder builder)
    {
        builder.AddSqlServerDbContext<AppDbContext>("product-db",
            configureDbContextOptions: cfg =>
            {
                cfg.UseAsyncSeeding(async (ctx,  _, ct) =>
                {
                    await ctx.SeedAsync<IAssemblyMarker>(ct);
                });
            });
    }

    private static void AddMediatR(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediatR(c =>
        {
            c.RegisterServicesFromAssemblyContaining<IAssemblyMarker>();
            c.AddOpenBehavior(typeof(TransactionBehaviour<,>));
        });
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

    private static void AddAuthorization(this IHostApplicationBuilder builder)
    {
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("DeleteBrandPolicy", policy => policy.Requirements.Add(new PermissionRequirement("Brand:Delete")))
            .AddPolicy("CreateBrandPolicy", policy => policy.Requirements.Add(new PermissionRequirement("Brand:Create")))
            .AddPolicy("UpdateBrandPolicy", policy => policy.Requirements.Add(new PermissionRequirement("Brand:Update")))
            .AddPolicy("DeleteSupplierPolicy", policy => policy.Requirements.Add(new PermissionRequirement("supplier:Delete")))
            .AddPolicy("CreateSupplierPolicy", policy => policy.Requirements.Add(new PermissionRequirement("supplier:Create")))
            .AddPolicy("UpdateSupplierPolicy", policy => policy.Requirements.Add(new PermissionRequirement("supplier:Update")));
    }
}