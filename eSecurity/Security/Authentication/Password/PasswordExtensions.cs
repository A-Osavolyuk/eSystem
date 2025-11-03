namespace eSecurity.Security.Authentication.Password;

public static class PasswordExtensions
{
    public static void AddPasswordManagement(this IServiceCollection services)
    {
        services.AddScoped<IPasswordManager, PasswordManager>();
    }
}