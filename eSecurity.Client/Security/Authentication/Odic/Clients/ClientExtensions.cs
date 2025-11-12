namespace eSecurity.Client.Security.Authentication.Odic.Clients;

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