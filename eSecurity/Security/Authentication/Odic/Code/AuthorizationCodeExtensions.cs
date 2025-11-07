namespace eSecurity.Security.Authentication.Odic.Code;

public static class AuthorizationCodeExtensions
{
    public static void AddAuthorizationCodeManagement(this IServiceCollection services)
    {
        services.AddScoped<IAuthorizationCodeManager, AuthorizationCodeManager>();
    }
}