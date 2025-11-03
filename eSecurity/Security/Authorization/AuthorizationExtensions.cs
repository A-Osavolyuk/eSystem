using eSecurity.Security.Authorization.Access;
using eSecurity.Security.Authorization.Devices;
using eSecurity.Security.Authorization.OAuth;
using eSecurity.Security.Authorization.Permissions;
using eSecurity.Security.Authorization.Policies;
using eSecurity.Security.Authorization.Roles;

namespace eSecurity.Security.Authorization;

public static class AuthorizationExtensions
{
    public static void AddAuthorization(this IHostApplicationBuilder builder)
    {
        builder.Services.AddPolicies();
        builder.Services.AddRoleManagement();
        builder.Services.AddAccessManagement();
        builder.Services.AddDeviceManagement();
        builder.Services.AddOAuthAuthorization();
        builder.Services.AddPermissionManagement();
    }
}