namespace eSecurity.Client.Security.Authentication.Odic.Clients;

public static class ClientExtensions
{
    public static void AddClient(this IServiceCollection services, Action<ClientOptions> configure)
    {
        services.Configure(configure);
    }
}