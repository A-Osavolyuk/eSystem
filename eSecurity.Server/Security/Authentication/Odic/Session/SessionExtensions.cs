namespace eSecurity.Server.Security.Authentication.Odic.Session;

public static class SessionExtensions
{
    public static void AddSession(this IServiceCollection services, Action<SessionOptions> configure)
    {
        services.AddScoped<ISessionManager, SessionManager>();
        services.Configure(configure);
    }
}