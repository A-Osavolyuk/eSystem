using eSecurity.Security.Authentication.Odic.Client;
using eSecurity.Security.Authentication.Odic.Code;
using eSecurity.Security.Authentication.Odic.PKCE;
using eSecurity.Security.Authentication.Odic.Session;
using eSecurity.Security.Authentication.Odic.Token;
using eSecurity.Security.Authentication.Odic.Token.Strategies;
using eSystem.Core.Security.Authentication.ODIC.Constants;

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
        services.AddKeyedScoped<TokenStrategy, AuthorizationCodeStrategy>(GrantTypes.AuthorizationCode);
        services.AddKeyedScoped<TokenStrategy, RefreshTokenStrategy>(GrantTypes.RefreshToken);
    }
}