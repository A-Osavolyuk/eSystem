using eSecurity.Security.Authentication.Jwt.Claims;
using eSecurity.Security.Cryptography.Tokens.Jwt;
using eSystem.Core.Security.Authentication.Jwt;

namespace eSecurity.Security.Authentication.Jwt;

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