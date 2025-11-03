namespace eAccount.Security.Authentication.ODIC.Clients;

public static class ClientExtensions
{
    public static void ConfigureClient(this IServiceCollection services, Action<ClientOptions> configure)
    {
        services.Configure(configure);
    }
}