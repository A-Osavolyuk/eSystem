namespace eSecurity.Client.Security.Authentication.Oidc.Clients;

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