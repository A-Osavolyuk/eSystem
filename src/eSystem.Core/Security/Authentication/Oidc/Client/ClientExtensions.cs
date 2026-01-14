using Microsoft.Extensions.DependencyInjection;

namespace eSystem.Core.Security.Authentication.Oidc.Client;

public static class ClientExtensions
{
    extension(IServiceCollection services)
    {
        public void AddClient(Action<ClientOptions> configure)
        {
            services.Configure(configure);
        }
    }
}