using eShop.Domain.Abstraction.State;
using eShop.Domain.DTOs;

namespace eShop.Infrastructure.State;

public class UserState : AsyncState
{
    public Guid UserId { get; set; }
    
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? RecoveryEmail { get; set; }

    public List<RoleDto> Roles { get; set; } = [];
    public List<PermissionDto> Permissions { get; set; } = [];
    
    public bool IsAuthenticated => UserId != Guid.Empty;

    public void LogOut()
    {
        UserId = Guid.Empty;
        Username = null;
        Email = null;
        RecoveryEmail = null;
        PhoneNumber = null;
        Roles.Clear();
        Permissions.Clear();
    }

    public void Map(UserStateDto state)
    {
        UserId = state.UserId;
        Email = state.Email;
        RecoveryEmail = state.RecoveryEmail;
        Username = state.Username;
        PhoneNumber = state.PhoneNumber;
        Permissions = state.Permissions;
        Roles = state.Roles;
    }

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
    
    public override async Task Change()
    {
        await StateChanged();
    }
}