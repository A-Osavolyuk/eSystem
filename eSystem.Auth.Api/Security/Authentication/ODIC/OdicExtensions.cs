using eSystem.Auth.Api.Security.Authentication.ODIC.Client;
using eSystem.Auth.Api.Security.Authentication.ODIC.Code;
using eSystem.Auth.Api.Security.Authentication.ODIC.PKCE;
using eSystem.Auth.Api.Security.Authentication.ODIC.Session;
using eSystem.Auth.Api.Security.Authentication.ODIC.Token;
using eSystem.Auth.Api.Security.Authentication.ODIC.Token.Strategies;
using eSystem.Core.Security.Authentication.ODIC.Token;

namespace eSystem.Auth.Api.Security.Authentication.ODIC;

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