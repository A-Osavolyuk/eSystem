using eSecurity.Security.Authentication.Odic.Client;
using eSecurity.Security.Authentication.Odic.Code;
using eSecurity.Security.Authentication.Odic.Logout;
using eSecurity.Security.Authentication.Odic.Logout.Strategies;
using eSecurity.Security.Authentication.Odic.Pkce;
using eSecurity.Security.Authentication.Odic.Session;
using eSecurity.Security.Authentication.Odic.Token;
using eSecurity.Security.Authentication.Odic.Token.Strategies;
using eSecurity.Security.Cryptography.Tokens.Jwt;
using eSecurity.Security.Identity.Claims;
using eSystem.Core.Security.Authentication.Jwt;
using eSystem.Core.Security.Authentication.Odic.Constants;

namespace eSecurity.Security.Authentication.Odic;

public static class OdicExtensions
{
    public static void AddOdic(this IServiceCollection services)
    {
        services.AddSession(cfg =>
        {
            cfg.Timestamp = TimeSpan.FromDays(30);
        });
        
        services.AddScoped<IClientManager, ClientManager>();
        services.AddScoped<IAuthorizationCodeManager, AuthorizationCodeManager>();
        services.AddScoped<IPkceHandler, PkceHandler>();
        services.AddScoped<ITokenStrategyResolver, TokenStrategyResolver>();
        services.AddKeyedScoped<ITokenStrategy, AuthorizationCodeStrategy>(GrantTypes.AuthorizationCode);
        services.AddKeyedScoped<ITokenStrategy, RefreshTokenStrategy>(GrantTypes.RefreshToken);

        services.AddScoped<ILogoutStrategyResolver, LogoutStrategyResolver>();
        services.AddKeyedScoped<ILogoutStrategy, ManualLogoutStrategy>(LogoutType.Manual);
        
        services.AddScoped<ITokenManager, TokenManager>();
        services.AddSingleton<IClaimBuilderFactory, ClaimBuilderFactory>();
    }
}