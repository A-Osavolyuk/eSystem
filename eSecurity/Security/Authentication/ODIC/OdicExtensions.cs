using eSecurity.Security.Authentication.ODIC.Client;
using eSecurity.Security.Authentication.ODIC.Code;
using eSecurity.Security.Authentication.ODIC.PKCE;
using eSecurity.Security.Authentication.ODIC.Token;
using eSecurity.Security.Authentication.ODIC.Token.Strategies;
using eSecurity.Security.Authentication.ODIC.Session;
using eSystem.Core.Security.Authentication.ODIC.Constants;

namespace eSecurity.Security.Authentication.ODIC;

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