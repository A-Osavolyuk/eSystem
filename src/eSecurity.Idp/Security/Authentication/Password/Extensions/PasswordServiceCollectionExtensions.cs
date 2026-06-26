namespace eSecurity.Idp.Security.Authentication.Password.Extensions;

public static class PasswordServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddPasswordManagement()
        {
            services.AddScoped<IPasswordQueryService, PasswordQueryService>();
            services.AddScoped<IPasswordCommandService, PasswordCommandService>();
        }
    }
}