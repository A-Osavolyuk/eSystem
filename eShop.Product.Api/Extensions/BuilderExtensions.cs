using eShop.Product.Api.Services;

namespace eShop.Product.Api.Extensions;

public static class BuilderExtensions
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
        builder.Services.AddDbContext<AppDbContext>(cfg =>
        {
            cfg.UseSqlServer(builder.Configuration["Configuration:Storage:Databases:SQL:MSSQL:ConnectionString"]!);
        });
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddMediatR(c =>
        {
            c.RegisterServicesFromAssemblyContaining<IAssemblyMarker>();
            c.AddOpenBehavior(typeof(LoggingBehaviour<,>), ServiceLifetime.Transient);
            c.AddOpenBehavior(typeof(TransactionBehaviour<,>), ServiceLifetime.Transient);
        });
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
        builder.Services.AddOpenApi();
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