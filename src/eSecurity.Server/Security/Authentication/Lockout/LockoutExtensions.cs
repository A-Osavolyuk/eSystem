namespace eSecurity.Server.Security.Authentication.Lockout;

public static class LockoutExtensions
{
    extension(IServiceCollection services)
    {
        public void AddLockout()
        {
            services.AddScoped<ILockoutManager, LockoutManager>();
        }
    }
}