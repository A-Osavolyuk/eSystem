namespace eSecurity.Server.Security.Authentication.Odic.Session;

public static class SessionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddSession(Action<SessionOptions> configure)
        {
            services.AddScoped<ISessionManager, SessionManager>();
            services.Configure(configure);
        }
    }
}