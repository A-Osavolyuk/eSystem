namespace eSecurity.Server.Security.Authentication.Odic.Configuration;

public static class OpenidConfigurationExtensions
{
    public static void AddOpenidConfiguration(this IServiceCollection services, Action<OpenidConfiguration> configure)
    {
        services.Configure(configure);
    }
}