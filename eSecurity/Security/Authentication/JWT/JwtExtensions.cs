using eSecurity.Security.Authentication.JWT.Claims;
using eSystem.Core.Security.Authentication.JWT;

namespace eSecurity.Security.Authentication.JWT;

public static class JwtExtensions
{
    public static void AddJwt(this IServiceCollection services)
    {
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        
        services.AddScoped<ITokenFactory, JwtTokenFactory>();
        services.AddScoped<ITokenManager, TokenManager>();
        services.AddSingleton<IClaimBuilderFactory, ClaimBuilderFactory>();
        
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
    }
}