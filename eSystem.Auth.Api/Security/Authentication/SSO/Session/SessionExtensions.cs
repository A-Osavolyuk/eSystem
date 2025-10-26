namespace eSystem.Auth.Api.Security.Authentication.SSO.Session;

public static class SessionExtensions
{
    public static void AddSession(this IHostApplicationBuilder builder, Action<SessionOptions> configure)
    {
        builder.Services.AddScoped<ISessionManager, SessionManager>();
        builder.Services.Configure(configure);
    }
}