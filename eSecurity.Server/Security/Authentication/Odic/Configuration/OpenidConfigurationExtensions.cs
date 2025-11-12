namespace eSecurity.Server.Security.Authentication.Odic.Configuration;

public static class OpenidConfigurationExtensions
{
    public static void AddOpenidConfiguration(this IServiceCollection services, Action<OpenIdOptions> configure)
    {
        services.Configure(configure);
    }
}