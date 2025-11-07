using eSecurity.Security.Authentication.Odic.Client;
using eSecurity.Security.Authentication.Odic.Code;
using eSecurity.Security.Authentication.Odic.Logout;
using eSecurity.Security.Authentication.Odic.Pkce;
using eSecurity.Security.Authentication.Odic.Session;
using eSecurity.Security.Authentication.Odic.Token;

namespace eSecurity.Security.Authentication.Odic;

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