namespace eSecurity.Server.Security.Authentication.Lockout;

public static class LockoutExtensions
{
    public static void AddLockout(this IServiceCollection services)
    {
        services.AddScoped<ILockoutManager, LockoutManager>();
    }
}