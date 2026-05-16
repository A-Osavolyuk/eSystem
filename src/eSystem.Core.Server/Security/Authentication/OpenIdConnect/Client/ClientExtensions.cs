using Microsoft.Extensions.DependencyInjection;

namespace eSystem.Core.Server.Security.Authentication.OpenIdConnect.Client;

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