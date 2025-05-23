using eShop.Domain.Interfaces.API;
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
        
        builder.Services.AddControllers();
    }

    private static void AddMsSqlDb(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString(SqlDb.SqlServer);
        builder.Services.AddDbContext<AppDbContext>(cfg =>
        {
            cfg.UseSqlServer(connectionString);
        });
    }

    private static void AddMediatR(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediatR(c =>
        {
            c.RegisterServicesFromAssemblyContaining<IAssemblyMarker>();
            c.AddOpenBehavior(typeof(TransactionBehaviour<,>), ServiceLifetime.Transient);
        });
    }
    private static void AddDependencyInjection(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ICacheService, CacheService>();

        builder.Services.AddScoped<AuthClient>();
    }

    private static void AddMessageBus(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var uri = builder.Configuration["Configuration:Services:MessageBus:RabbitMq:HostUri"]!;
                var username = builder.Configuration["Configuration:Services:MessageBus:RabbitMq:UserName"]!;
                var password = builder.Configuration["Configuration:Services:MessageBus:RabbitMq:Password"]!;

                cfg.Host(new Uri(uri), h =>
                {
                    h.Username(username);
                    h.Password(password);
                });
            });
        });
    }
}