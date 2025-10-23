using eSystem.Application.Security.Authorization.Requirements;

namespace eSystem.Auth.Api.Security.Authorization;

public static class AuthorizationExtensions
{
    public static void AddAuthorization(this IHostApplicationBuilder builder)
    {
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("DeleteAccountPolicy", policy => policy.Requirements.Add(new PermissionRequirement("DELETE_ACCOUNT")))
            .AddPolicy("CreateAccountPolicy", policy => policy.Requirements.Add(new PermissionRequirement("CREATE_ACCOUNT")))
            .AddPolicy("UpdateAccountPolicy", policy => policy.Requirements.Add(new PermissionRequirement("UPDATE_ACCOUNT")))
            .AddPolicy("ReadAccountPolicy", policy => policy.Requirements.Add(new PermissionRequirement("READ_ACCOUNT")))
            .AddPolicy("DeleteUserPolicy", policy => policy.Requirements.Add(new PermissionRequirement("DELETE_USER")))
            .AddPolicy("CreateUserPolicy", policy => policy.Requirements.Add(new PermissionRequirement("CREATE_USER")))
            .AddPolicy("UpdateUserPolicy", policy => policy.Requirements.Add(new PermissionRequirement("UPDATE_USER")))
            .AddPolicy("ReadUserPolicy", policy => policy.Requirements.Add(new PermissionRequirement("READ_USER")))
            .AddPolicy("LockoutUserPolicy", policy => policy.Requirements.Add(new PermissionRequirement("LOCKOUT_USER")))
            .AddPolicy("UnlockUserPolicy", policy => policy.Requirements.Add(new PermissionRequirement("UNLOCK_USER")))
            .AddPolicy("DeleteRolePolicy", policy => policy.Requirements.Add(new PermissionRequirement("DELETE_ROLE")))
            .AddPolicy("CreateRolePolicy", policy => policy.Requirements.Add(new PermissionRequirement("CREATE_ROLE")))
            .AddPolicy("UpdateRolePolicy", policy => policy.Requirements.Add(new PermissionRequirement("UPDATE_ROLE")))
            .AddPolicy("ReadRolePolicy", policy => policy.Requirements.Add(new PermissionRequirement("READ_ROLE")))
            .AddPolicy("AssignRolePolicy", policy => policy.Requirements.Add(new PermissionRequirement("ASSIGN_ROLE")))
            .AddPolicy("UnassignRolePolicy", policy => policy.Requirements.Add(new PermissionRequirement("UNASSIGN_ROLE")))
            .AddPolicy("DeletePermissionPolicy", policy => policy.Requirements.Add(new PermissionRequirement("DELETE_PERMISSION")))
            .AddPolicy("CreatePermissionPolicy", policy => policy.Requirements.Add(new PermissionRequirement("CREATE_PERMISSION")))
            .AddPolicy("UpdatePermissionPolicy", policy => policy.Requirements.Add(new PermissionRequirement("UPDATE_PERMISSION")))
            .AddPolicy("ReadPermissionPolicy", policy => policy.Requirements.Add(new PermissionRequirement("READ_PERMISSIONS")))
            .AddPolicy("GrantPermissionPolicy", policy => policy.Requirements.Add(new PermissionRequirement("GRANT_PERMISSIONS")))
            .AddPolicy("RevokePermissionPolicy", policy => policy.Requirements.Add(new PermissionRequirement("REVOKE_PERMISSIONS")));
    }
}