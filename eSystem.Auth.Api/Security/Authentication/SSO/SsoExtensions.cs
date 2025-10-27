using eSystem.Auth.Api.Security.Authentication.SSO.Client;
using eSystem.Auth.Api.Security.Authentication.SSO.Code;
using eSystem.Auth.Api.Security.Authentication.SSO.PKCE;
using eSystem.Auth.Api.Security.Authentication.SSO.Session;

namespace eSystem.Auth.Api.Security.Authentication.SSO;

public static class SsoExtensions
{
    public static void AddSSO(this IHostApplicationBuilder builder)
    {
        builder.AddSession(cfg =>
        {
            cfg.Timestamp = TimeSpan.FromDays(30);
        });
        
        builder.Services.AddScoped<IClientManager, ClientManager>();
        builder.Services.AddScoped<IAuthorizationCodeManager, AuthorizationCodeManager>();
        builder.Services.AddScoped<IPkceHandler, PkceHandler>();
    }
}