using eSystem.Auth.Api.Security.Cryptography.Tokens;
using eSystem.Core.Security.Authentication.JWT;

namespace eSystem.Auth.Api.Security.Authentication.JWT;

public static class JwtExtensions
{
    public static void AddJwt(this IServiceCollection services)
    {
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

        services.AddScoped<ITokenFactory, JwtTokenFactory>();
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
    }
}