namespace eSecurity.Server.Security.Authentication.Odic.Code;

public static class AuthorizationCodeExtensions
{
    extension(IServiceCollection services)
    {
        public void AddAuthorizationCodeManagement()
        {
            services.AddScoped<IAuthorizationCodeManager, AuthorizationCodeManager>();
        }
    }
}