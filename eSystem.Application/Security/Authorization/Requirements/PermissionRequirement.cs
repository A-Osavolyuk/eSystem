using eSystem.Domain.Security.Claims;
using eSystem.Application.Attributes;
using Microsoft.AspNetCore.Authorization;

namespace eSystem.Application.Security.Authorization.Requirements;

public class PermissionRequirement(string permissionName) : IAuthorizationRequirement
{
    public string PermissionName { get; } = permissionName;
}

[Injectable(typeof(IAuthorizationHandler), ServiceLifetime.Singleton)]
public sealed class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.HasClaim(c => c.Type == AppClaimTypes.Permission && c.Value == requirement.PermissionName) 
            || context.User.IsInRole("Admin") || HasAll(requirement.PermissionName))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

    private bool HasAll(string permissionName)
    {
        var permissionNameParts = permissionName.Split('_');
        var resource = permissionNameParts[0];
        var allAccessPermission = $"{resource}:ALL";
        return permissionName == allAccessPermission;
    }
}