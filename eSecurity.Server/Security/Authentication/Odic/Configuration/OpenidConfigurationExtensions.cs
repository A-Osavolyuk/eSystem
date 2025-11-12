namespace eSecurity.Server.Security.Authentication.Odic.Configuration;

public static class OpenidConfigurationExtensions
{
    extension(IServiceCollection services)
    {
        public void AddOpenidConfiguration(Action<OpenIdOptions> configure)
        {
            services.Configure(configure);
        }
    }
}