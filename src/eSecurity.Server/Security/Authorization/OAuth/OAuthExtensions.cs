using eSecurity.Server.Security.Authorization.OAuth.LinkedAccount;

namespace eSecurity.Server.Security.Authorization.OAuth;

public static class OAuthExtensions
{
    extension(IServiceCollection services)
    {
        public void AddOAuthAuthorization()
        {
            services.AddScoped<ILinkedAccountManager, LinkedAccountManager>();
        }
    }
}