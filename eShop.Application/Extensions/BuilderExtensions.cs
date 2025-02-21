using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace eShop.Application.Extensions;

public static class BuilderExtensions
{
    public static void AddValidation(this IHostApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining(typeof(BuilderExtensions));
    }

    public static void AddVersioning(this IHostApplicationBuilder builder)
    {
        builder.Services.AddApiVersioning(options =>
        {
            const string key = "api-version";
            
            options.ReportApiVersions = true;
            options.DefaultApiVersion = ApiVersion.Default;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new QueryStringApiVersionReader(key),
                new HeaderApiVersionReader(key));
        });

        builder.Services.AddVersionedApiExplorer(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = ApiVersion.Default;
            options.SubstituteApiVersionInUrl = true;
            options.GroupNameFormat = "'v'V";
        });
    }

    public static void AddJwtAuthentication(this IHostApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            const string audiencePath = "Configuration:Security:Authentication:JWT:Audience";
            const string issuerPath = "Configuration:Security:Authentication:JWT:Issuer";
            const string keyPath = "Configuration:Security:Authentication:JWT:Key";
            
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidAudience = builder.Configuration[audiencePath],
                ValidIssuer = builder.Configuration[issuerPath],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration[keyPath]!))
            };
        });
    }

    public static void AddRedisCache(this IHostApplicationBuilder builder)
    {
        const string connectionStringPath = "Configuration:Services:Cache:Redis:ConnectionString";
        const string instanceNamePath = "Configuration:Services:Cache:Redis:InstanceName";
        
        var connectionString = builder.Configuration[connectionStringPath]!;
        var instanceName = builder.Configuration[instanceNamePath]!;
        
        builder.Services.AddSingleton<IConnectionMultiplexer>(sp => 
                ConnectionMultiplexer.Connect(connectionString));
        
        builder.Services.AddStackExchangeRedisCache(cfg =>
        {
            cfg.Configuration = connectionString;
            cfg.InstanceName = instanceName;
        });
    }
    
    public static void AddLogging(this IHostApplicationBuilder builder)
    {
        const string key = "Configuration:Logging";
        builder.Logging.AddConfiguration(builder.Configuration.GetSection(key));
    }
}