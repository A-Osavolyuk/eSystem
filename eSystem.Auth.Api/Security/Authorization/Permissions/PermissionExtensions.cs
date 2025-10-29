using eSystem.Core.Security.Authorization.Requirements;

namespace eSystem.Auth.Api.Security.Authorization.Permissions;

public static class PermissionExtensions
{
    public static void AddPermissionManagement(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
        services.AddScoped<IPermissionManager, PermissionManager>();
    }
}