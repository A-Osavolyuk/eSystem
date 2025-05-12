namespace eShop.Auth.Api.Security.Authorization;

public class PermissionRequirement(string permissionName) : IAuthorizationRequirement
{
    public string PermissionName { get; } = permissionName;
}

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.HasClaim(c => c.Type == ClaimTypes.Permission && c.Value == requirement.PermissionName) 
            || (context.User.HasClaim(c => c is { Type: ClaimTypes.Permission, Value: "Admin:All" })
                && context.User.IsInRole("Admin")))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}