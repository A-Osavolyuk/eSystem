namespace eSecurity.Idp.Security.Authorization.Roles;

public static class RolesExtensions
{
    extension(IServiceCollection services)
    {
        public void AddRoleManagement()
        {
            services.AddScoped<IRoleManager, RoleManager>();
        }
    }
}