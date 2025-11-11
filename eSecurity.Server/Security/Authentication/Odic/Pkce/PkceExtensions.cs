namespace eSecurity.Server.Security.Authentication.Odic.Pkce;

public static class PkceExtensions
{
    public static void AddPkceHandler(this IServiceCollection services)
    {
        services.AddScoped<IPkceHandler, PkceHandler>();
    }
}