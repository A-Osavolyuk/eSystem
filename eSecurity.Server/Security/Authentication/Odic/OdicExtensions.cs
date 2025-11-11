using eSecurity.Server.Security.Authentication.Odic.Client;
using eSecurity.Server.Security.Authentication.Odic.Code;
using eSecurity.Server.Security.Authentication.Odic.Logout;
using eSecurity.Server.Security.Authentication.Odic.Pkce;
using eSecurity.Server.Security.Authentication.Odic.Session;
using eSecurity.Server.Security.Authentication.Odic.Token;

namespace eSecurity.Server.Security.Authentication.Odic;

public static class OdicExtensions
{
    public static void AddOdic(this IServiceCollection services)
    {
        services.AddPkceHandler();
        services.AddLogoutFlow();
        services.AddTokenFlow();
        services.AddClientManagement();
        services.AddAuthorizationCodeManagement();
        services.AddSession(cfg =>
        {
            cfg.Timestamp = TimeSpan.FromDays(30);
        });
    }
}