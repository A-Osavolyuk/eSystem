using eShop.Domain.DTOs;

namespace eShop.Infrastructure.State;

public class UserIdentity
{
    public List<RoleDto> Roles { get; set; } = [];
    public List<PermissionDto> Permissions { get; set; } = [];
    
    public bool HasRole(string role)
    {
        return Roles.Any(x => x.Name == role);
    }

    public bool HasRole(List<string> roles)
    {
        return roles.Intersect(Roles.Select(x => x.Name)).Any();
    }

    public bool HasPermission(string permission)
    {
        return Permissions.Any(x => x.Name == permission);
    }

    public bool HasPermission(List<string> permissions)
    {
        return permissions.Intersect(Permissions.Select(x => x.Name)).Any();
    }
}