using eSystem.Auth.Api.Security.Authorization.Access;
using eSystem.Auth.Api.Security.Authorization.Devices;
using eSystem.Auth.Api.Security.Authorization.OAuth;
using eSystem.Auth.Api.Security.Authorization.Permissions;
using eSystem.Auth.Api.Security.Authorization.Policies;
using eSystem.Auth.Api.Security.Authorization.Roles;
using eSystem.Core.Security.Authorization.Requirements;

namespace eSystem.Auth.Api.Security.Authorization;

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