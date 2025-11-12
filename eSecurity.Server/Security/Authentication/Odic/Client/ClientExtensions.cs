namespace eSecurity.Server.Security.Authentication.Odic.Client;

public static class ClientExtensions
{
    extension(IServiceCollection services)
    {
        public void AddClientManagement()
        {
            services.AddScoped<IClientManager, ClientManager>();
        }
    }
}