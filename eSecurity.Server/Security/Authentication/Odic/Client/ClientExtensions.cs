namespace eSecurity.Server.Security.Authentication.Odic.Client;

public static class ClientExtensions
{
    public static void AddClientManagement(this IServiceCollection services)
    {
        services.AddScoped<IClientManager, ClientManager>();
    }
}