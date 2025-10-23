using eSystem.Auth.Api.Security.Tokens.Jwt;

namespace eSystem.Auth.Api.Security.Tokens;

public static class TokenExtensions
{
    public static void AddTokens(this IHostApplicationBuilder builder)
    {
        builder.Services.AddJwt();
    }
    
    private static void AddJwt(this IServiceCollection services)
    {
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

        services.AddScoped<ITokenFactory, JwtTokenFactory>();
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
    }
}