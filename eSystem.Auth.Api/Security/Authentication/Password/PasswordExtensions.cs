namespace eSystem.Auth.Api.Security.Authentication.Password;

public static class PasswordExtensions
{
    public static void AddPasswordManagement(this IServiceCollection services)
    {
        services.AddScoped<IPasswordManager, PasswordManager>();
    }
}