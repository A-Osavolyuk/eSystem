using eShop.Auth.Api.Security.Tokens.Jwt;

namespace eShop.Auth.Api.Security.Tokens;

public static class TokenExtensions
{
    public static void AddTokens(this IHostApplicationBuilder builder)
    {
        builder.AddTokens();
    }
    
    private static void AddJwt(this IServiceCollection services)
    {
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

        services.AddScoped<ITokenFactory, JwtTokenFactory>();
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
    }
}