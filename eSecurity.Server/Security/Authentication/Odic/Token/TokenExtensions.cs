using eSecurity.Server.Security.Authentication.Odic.Token.Strategies;
using eSecurity.Server.Security.Identity.Claims;
using eSystem.Core.Security.Authentication.Odic.Constants;

namespace eSecurity.Server.Security.Authentication.Odic.Token;

public static class TokenExtensions
{
    public static void AddTokenFlow(this IServiceCollection services)
    {
        services.AddScoped<ITokenManager, TokenManager>();
        services.AddSingleton<IClaimBuilderFactory, ClaimBuilderFactory>();
        services.AddScoped<ITokenStrategyResolver, TokenStrategyResolver>();
        services.AddKeyedScoped<ITokenStrategy, AuthorizationCodeStrategy>(GrantTypes.AuthorizationCode);
        services.AddKeyedScoped<ITokenStrategy, RefreshTokenStrategy>(GrantTypes.RefreshToken);
    }
}