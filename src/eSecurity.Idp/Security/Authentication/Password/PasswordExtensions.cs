namespace eSecurity.Idp.Security.Authentication.Password;

public static class PasswordExtensions
{
    extension(IServiceCollection services)
    {
        public void AddPasswordManagement()
        {
            services.AddScoped<IPasswordManager, PasswordManager>();
        }
    }
}