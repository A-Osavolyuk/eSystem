namespace eSecurity.Server.Security.Authentication.Odic.Pkce;

public static class PkceExtensions
{
    extension(IServiceCollection services)
    {
        public void AddPkceHandler()
        {
            services.AddScoped<IPkceHandler, PkceHandler>();
        }
    }
}