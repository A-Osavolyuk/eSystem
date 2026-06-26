namespace eSecurity.Idp.Security.Authentication.Session.Extensions;

public static class SessionServiceCollectionExtensions
{
    public static void AddSsoSessions(this IServiceCollection services, Action<SessionOptions> configure)
    {
        services.Configure(configure);
        
        services.AddScoped<ISessionQueryService, SessionQueryService>();
        services.AddScoped<ISessionCommandService, SessionCommandService>();
    }
}