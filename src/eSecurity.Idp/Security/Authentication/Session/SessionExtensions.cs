namespace eSecurity.Idp.Security.Authentication.Session;

public static class SessionExtensions
{
    public static void AddSsoSessions(this IServiceCollection services, Action<SessionOptions> configure)
    {
        services.AddScoped<ISessionManager, SessionManager>();
        services.Configure(configure);
    }
}