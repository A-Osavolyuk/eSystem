namespace eSecurity.Server.Security.Authorization.Consents;

public static class ConsentExtensions
{
    extension(IServiceCollection services)
    {
        public void AddConsentManagement()
        {
            services.AddScoped<IConsentManager, ConsentManager>();
        }
    }
}