using eSystem.Auth.Api.Security.Authentication.SSO.Client;
using eSystem.Auth.Api.Security.Authentication.SSO.Code;
using eSystem.Auth.Api.Security.Authentication.SSO.PKCE;
using eSystem.Auth.Api.Security.Authentication.SSO.Session;
using eSystem.Auth.Api.Security.Authentication.SSO.Token;
using eSystem.Auth.Api.Security.Authentication.SSO.Token.Strategies;
using eSystem.Core.Security.Authentication.SSO.Token;

namespace eSystem.Auth.Api.Security.Authentication.SSO;

public static class SsoExtensions
{
    public static void AddSSO(this IServiceCollection services)
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