using eSecurity.Client.Security.Authentication.Odic.Clients;
using eSystem.Core.Security.Authentication.Odic.Constants;

namespace eSecurity.Client.Security.Authentication.Odic;

public static class OdicExtensions
{
    public static void AddOdic(this IServiceCollection services)
    {
        services.AddClient(cfg =>
        {
            cfg.ClientId = "eSecurity";
            cfg.ClientSecret = "2f213a036e325a55dc19320f03c2fad7c13f0169788b5968686cb4931341c393a651d7e6";
            cfg.CallbackUri = "http://localhost:5501/connect/callback";
            cfg.PostLogoutRedirectUri = "http://localhost:5501/connect/logout/callback";
            cfg.Scopes =
            [
                Scopes.OpenId,
                Scopes.OfflineAccess,
                Scopes.Email,
                Scopes.Phone,
                Scopes.Profile,
                Scopes.Address
            ];
        });
    }
}