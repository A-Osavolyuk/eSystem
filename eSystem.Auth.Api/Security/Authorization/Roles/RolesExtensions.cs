namespace eSystem.Auth.Api.Security.Authorization.Roles;

public static class RolesExtensions
{
    public static void AddRoleManagement(this IServiceCollection services)
    {
        services.AddScoped<IRoleManager, RoleManager>();
    }
}