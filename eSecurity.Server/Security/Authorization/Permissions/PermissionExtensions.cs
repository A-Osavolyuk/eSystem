using eSystem.Core.Security.Authorization.Requirements;

namespace eSecurity.Server.Security.Authorization.Permissions;

public static class PermissionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddPermissionManagement()
        {
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
            services.AddScoped<IPermissionManager, PermissionManager>();
        }
    }
}