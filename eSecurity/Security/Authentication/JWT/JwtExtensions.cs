using eSystem.Core.Security.Authentication.JWT;
using eSystem.Core.Security.Cryptography.Tokens;

namespace eSecurity.Security.Authentication.JWT;

public static class JwtExtensions
{
    public static void AddJwt(this IServiceCollection services)
    {
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        
        services.AddScoped<ITokenFactory, JwtTokenFactory>();
        services.AddScoped<ITokenManager, TokenManager>();
        
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
    }
}