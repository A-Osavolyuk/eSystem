using eSecurity.Client.Security.Authentication.Oidc.Clients;
using eSecurity.Client.Security.Authentication.Oidc.Token;
using eSystem.Core.Security.Authentication.Oidc;

namespace eSecurity.Client.Security.Authentication.Oidc;

public static class OdicExtensions
{
    extension(IServiceCollection services)
    {
        public void AddOidc()
        {
            services.AddScoped<ITokenValidator, TokenValidator>();
            services.AddClient(cfg =>
            {
                cfg.ClientAudience = "eSecurity";
                cfg.ClientId = "392e390f-33bd-4f30-af70-ccbe04bbb2c4";
                cfg.ClientSecret = "2f213a036e325a55dc19320f03c2fad7c13f0169788b5968686cb4931341c393a651d7e6";
                cfg.CallbackUri = "https://localhost:6501/connect/callback";
                cfg.PostLogoutRedirectUri = "https://localhost:6501/connect/logout/callback";
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
}